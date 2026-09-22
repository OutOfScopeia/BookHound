package com.outofscopia.bookhound

import android.graphics.Rect
import com.google.mlkit.vision.text.Text
import java.text.Normalizer

/** Strip diacritics; used for both search modes so OCR noise on accents doesn't hurt matches. */
private fun stripDiacritics(s: String): String =
    Normalizer.normalize(s, Normalizer.Form.NFD)
        .replace("\\p{InCombiningDiacriticalMarks}+".toRegex(), "")

/** Lowercase + strip diacritics for accent- and case-insensitive matching. */
fun normalize(s: String): String = stripDiacritics(s).lowercase()

/** Strip diacritics only, preserving case. */
fun normalizeCaseSensitive(s: String): String = stripDiacritics(s)

data class HighlightMatch(val line: String, val box: Rect)

/**
 * Locate every occurrence of [query] inside [text] and return a tight bounding rect for each.
 * Walks symbols so the box covers only the matched glyphs, not the whole line/word.
 * When [caseSensitive] is true, matches preserve letter case (diacritics are still stripped).
 */
fun findHighlights(text: Text?, query: String, caseSensitive: Boolean = false): List<HighlightMatch> {
    if (text == null || query.isBlank()) return emptyList()
    val norm: (String) -> String = if (caseSensitive) ::normalizeCaseSensitive else ::normalize
    val nq = norm(query)
    if (nq.isEmpty()) return emptyList()

    val out = ArrayList<HighlightMatch>()
    for (block in text.textBlocks) for (line in block.lines) {
        // Flatten the line into (char, box) pairs. Boxes may be null for inter-word gaps
        // and are excluded from unions.
        val chars = ArrayList<Char>(line.text.length + 4)
        val boxes = ArrayList<Rect?>(line.text.length + 4)
        for ((ei, elem) in line.elements.withIndex()) {
            if (ei > 0) { chars.add(' '); boxes.add(null) }
            val syms = elem.symbols
            if (syms.isEmpty()) {
                val ns = norm(elem.text)
                for (c in ns) { chars.add(c); boxes.add(elem.boundingBox) }
            } else {
                for (sym in syms) {
                    val ns = norm(sym.text)
                    for (c in ns) { chars.add(c); boxes.add(sym.boundingBox) }
                }
            }
        }
        if (chars.isEmpty()) continue
        val flat = String(chars.toCharArray())
        var from = 0
        while (true) {
            val idx = flat.indexOf(nq, from)
            if (idx < 0) break
            val end = idx + nq.length
            val union = boxes.subList(idx, end).filterNotNull()
            if (union.isNotEmpty()) {
                val r = Rect(union[0])
                for (k in 1 until union.size) r.union(union[k])
                out.add(HighlightMatch(flat.substring(idx, end), r))
            }
            from = end
        }
    }
    return out
}

