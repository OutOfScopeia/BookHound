package dev.bookhound.kmp

import android.Manifest
import android.content.pm.PackageManager
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.core.content.ContextCompat

@Composable
fun BookHoundApp() {
    val context = LocalContext.current
    MaterialTheme(colorScheme = dynamicDarkColorScheme(context)) {
        Surface(color = Color.Black, modifier = Modifier.fillMaxSize()) {
            var hasCamera by remember {
                mutableStateOf(
                    ContextCompat.checkSelfPermission(context, Manifest.permission.CAMERA)
                        == PackageManager.PERMISSION_GRANTED
                )
            }
            val request = rememberLauncherForActivityResult(
                ActivityResultContracts.RequestPermission()
            ) { granted -> hasCamera = granted }

            LaunchedEffect(Unit) {
                if (!hasCamera) request.launch(Manifest.permission.CAMERA)
            }

            if (hasCamera) {
                CameraOcrScreen()
            } else {
                Box(Modifier.fillMaxSize().background(Color.Black), Alignment.Center) {
                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                        Text(
                            "Camera permission required",
                            color = Color.White,
                            fontWeight = FontWeight.SemiBold
                        )
                        Spacer(Modifier.height(12.dp))
                        Button(onClick = { request.launch(Manifest.permission.CAMERA) }) {
                            Text("Grant")
                        }
                    }
                }
            }
        }
    }
}
