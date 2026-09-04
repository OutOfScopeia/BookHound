# BookHound

A native Android app that opens the phone camera, runs on-device OCR on the
live viewfinder, and buzzes when the user-entered search string appears — with
the match highlighted by a yellow overlay tightly wrapping the matched
characters. Designed to help you locate a specific book on a library shelf by
its classification code / call number without scanning individual spines by
eye.

Inspired by the reader-service desks of the Cambridge University Library.

## Stack

| Concern | Choice |
| ------- | ------ |
| Language | Kotlin 2.0.21 |
| UI | Compose Multiplatform 1.7.3 (single `androidTarget`) |
| Camera | CameraX 1.4.1 (`LifecycleCameraController` + `PreviewView`) |
| OCR | ML Kit Text Recognition 16.0.1 (Latin script, on-device) |
| Analyzer bridge | `androidx.camera.mlkit.vision:MlKitAnalyzer` |
| Build | Android Gradle Plugin 8.7.3 / Gradle 8.10.2 |
| Min / target SDK | 31 (Android 12) / 35 |
| Theming | Material 3 with Material You dynamic colors |

Everything runs on-device — camera frames are analyzed locally and never
leave the phone.

## Project layout

```
.
├── app/
│   ├── build.gradle.kts               # Android application config, signing, R8
│   ├── proguard-rules.pro              # ML Kit / CameraX keep rules
│   └── src/androidMain/
│       ├── AndroidManifest.xml         # CAMERA + VIBRATE, adaptive icon
│       ├── kotlin/dev/bookhound/kmp/
│       │   ├── MainActivity.kt         # single-activity host
│       │   ├── BookHoundApp.kt         # MaterialTheme + dynamic colors
│       │   ├── CameraOcrScreen.kt      # CameraX preview + overlay + controls
│       │   └── Search.kt               # symbol-accurate highlight matcher
│       └── res/
│           ├── drawable/               # adaptive-icon layers
│           ├── mipmap-anydpi-v26/      # adaptive-icon spec
│           └── values/strings.xml
├── gradle/                             # version catalog + wrapper
├── settings.gradle.kts
├── build.gradle.kts
├── keystore.properties.template        # copy to keystore.properties for signing
└── RELEASE.md                          # step-by-step Play Store release guide
```

## Build & run

Requirements: JDK 17, Android SDK, a device with camera on Android 12+.

```powershell
$env:JAVA_HOME = 'C:\Program Files\Microsoft\jdk-17.0.20.101-hotspot'
.\gradlew.bat :app:installDebug
```

## Release to Google Play

See [RELEASE.md](RELEASE.md) for keystore generation, AAB build, and Play
Console setup.

