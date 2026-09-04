package dev.bookhound.kmp

import android.content.Context
import android.os.VibrationEffect
import android.os.VibratorManager
import androidx.camera.core.*
import androidx.camera.lifecycle.ProcessCameraProvider
import androidx.camera.mlkit.vision.MlKitAnalyzer
import androidx.camera.view.CameraController
import androidx.camera.view.LifecycleCameraController
import androidx.camera.view.PreviewView
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.detectTapGestures
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.FlashOff
import androidx.compose.material.icons.filled.FlashOn
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.drawscope.Stroke
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.viewinterop.AndroidView
import androidx.core.content.ContextCompat
import androidx.lifecycle.compose.LocalLifecycleOwner
import com.google.mlkit.vision.text.Text
import com.google.mlkit.vision.text.TextRecognition
import com.google.mlkit.vision.text.latin.TextRecognizerOptions
import java.util.concurrent.Executors

@Composable
fun CameraOcrScreen() {
    val context = LocalContext.current
    val lifecycleOwner = LocalLifecycleOwner.current

    var query by remember { mutableStateOf("") }
    var matches by remember { mutableStateOf<List<HighlightMatch>>(emptyList()) }
    var torchOn by remember { mutableStateOf(false) }
    var zoom by remember { mutableFloatStateOf(0f) }

    val queryLatest by rememberUpdatedState(query)
    val matchesEmptyLatest by rememberUpdatedState(matches.isEmpty())

    val controller = remember {
        LifecycleCameraController(context).apply {
            setEnabledUseCases(
                CameraController.IMAGE_ANALYSIS or CameraController.IMAGE_CAPTURE
            )
        }
    }

    val analyzerExecutor = remember { Executors.newSingleThreadExecutor() }
    val mainExecutor = remember { ContextCompat.getMainExecutor(context) }
    val recognizer = remember {
        TextRecognition.getClient(TextRecognizerOptions.DEFAULT_OPTIONS)
    }

    DisposableEffect(Unit) {
        onDispose {
            recognizer.close()
            analyzerExecutor.shutdown()
            controller.unbind()
        }
    }

    LaunchedEffect(controller, lifecycleOwner) {
        controller.bindToLifecycle(lifecycleOwner)
        controller.setImageAnalysisAnalyzer(
            analyzerExecutor,
            MlKitAnalyzer(
                listOf(recognizer),
                CameraController.COORDINATE_SYSTEM_VIEW_REFERENCED,
                mainExecutor
            ) { result ->
                val text: Text? = result?.getValue(recognizer)
                val newMatches = findHighlights(text, queryLatest)
                val wasEmpty = matchesEmptyLatest
                matches = newMatches
                if (newMatches.isNotEmpty() && wasEmpty) vibrate(context)
            }
        )
    }

    LaunchedEffect(torchOn) { controller.enableTorch(torchOn) }
    LaunchedEffect(zoom) { controller.setLinearZoom(zoom) }

    Box(Modifier.fillMaxSize().background(Color.Black)) {
        AndroidView(
            factory = { ctx ->
                PreviewView(ctx).apply {
                    this.controller = controller
                    scaleType = PreviewView.ScaleType.FILL_CENTER
                    implementationMode = PreviewView.ImplementationMode.PERFORMANCE
                }
            },
            modifier = Modifier
                .fillMaxSize()
                .pointerInput(controller) {
                    detectTapGestures { }
                }
        )

        Canvas(Modifier.fillMaxSize()) {
            matches.forEach { m ->
                val r = m.box
                val topLeft = Offset(r.left.toFloat(), r.top.toFloat())
                val sz = Size((r.right - r.left).toFloat(), (r.bottom - r.top).toFloat())
                drawRect(color = Color(0x66FFEB3B), topLeft = topLeft, size = sz)
                drawRect(
                    color = Color(0xFFFFEB3B),
                    topLeft = topLeft,
                    size = sz,
                    style = Stroke(width = 3f)
                )
            }
        }

        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp)
                .statusBarsPadding(),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            if (matches.isNotEmpty()) {
                Surface(
                    color = Color(0xCC1B5E20),
                    shape = RoundedCornerShape(12.dp),
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Column(Modifier.padding(12.dp)) {
                        Text(
                            "${matches.size} match${if (matches.size == 1) "" else "es"}",
                            color = Color.White,
                            fontWeight = FontWeight.Bold
                        )
                        matches.take(3).forEach {
                            Text("• ${it.line.trim()}", color = Color.White, maxLines = 1)
                        }
                    }
                }
            }
        }

        Column(
            modifier = Modifier
                .align(Alignment.BottomCenter)
                .fillMaxWidth()
                .padding(16.dp)
                .navigationBarsPadding(),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            Surface(
                color = Color(0xAA000000),
                shape = RoundedCornerShape(12.dp),
                modifier = Modifier.fillMaxWidth()
            ) {
                Row(
                    Modifier.padding(horizontal = 12.dp, vertical = 6.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text("Zoom", color = Color.White, modifier = Modifier.padding(end = 8.dp))
                    val brightPrimary = androidx.compose.ui.graphics.lerp(
                        MaterialTheme.colorScheme.primary, Color.White, 0.4f
                    )
                    Slider(
                        value = zoom,
                        onValueChange = { zoom = it },
                        modifier = Modifier.weight(1f),
                        colors = SliderDefaults.colors(
                            thumbColor = brightPrimary,
                            activeTrackColor = brightPrimary,
                            inactiveTrackColor = Color(0x66FFFFFF)
                        )
                    )
                    IconButton(onClick = { torchOn = !torchOn }) {
                        Icon(
                            if (torchOn) Icons.Filled.FlashOn else Icons.Filled.FlashOff,
                            contentDescription = "Torch",
                            tint = Color.White
                        )
                    }
                }
            }

            OutlinedTextField(
                value = query,
                onValueChange = { query = it },
                placeholder = { Text("Search text…") },
                singleLine = true,
                modifier = Modifier.fillMaxWidth(),
                colors = OutlinedTextFieldDefaults.colors(
                    focusedContainerColor = Color(0xCC000000),
                    unfocusedContainerColor = Color(0xAA000000),
                    focusedTextColor = Color.White,
                    unfocusedTextColor = Color.White,
                    cursorColor = Color.White,
                    focusedBorderColor = Color.White,
                    unfocusedBorderColor = Color(0xFFAAAAAA),
                    focusedPlaceholderColor = Color(0xFFCCCCCC),
                    unfocusedPlaceholderColor = Color(0xFFCCCCCC)
                )
            )
        }
    }
}

private fun vibrate(context: Context) {
    val mgr = context.getSystemService(Context.VIBRATOR_MANAGER_SERVICE) as VibratorManager
    mgr.defaultVibrator.vibrate(VibrationEffect.createOneShot(60L, VibrationEffect.DEFAULT_AMPLITUDE))
}
