# ML Kit — keep vision APIs and text recognizer internals used via reflection.
-keep class com.google.mlkit.** { *; }
-keep class com.google.android.gms.internal.mlkit_** { *; }
-keep interface com.google.mlkit.** { *; }
-dontwarn com.google.mlkit.**
-dontwarn com.google.android.gms.internal.mlkit_**

# CameraX MlKitAnalyzer bridge.
-keep class androidx.camera.mlkit.vision.** { *; }
-dontwarn androidx.camera.**

# Kotlin coroutines internal service loader stubs.
-dontwarn kotlinx.coroutines.debug.**

# Keep the launcher activity so intent resolution still works after R8 renaming.
-keep class dev.bookhound.kmp.MainActivity { *; }
