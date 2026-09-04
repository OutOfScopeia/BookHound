# Play Store release

## 1. Create the upload keystore (one-time)

Generate a **20+ year** RSA key. Store the `.jks` somewhere safe outside the repo (and back it up — losing this key means losing the ability to update the app on Play).

```powershell
$env:JAVA_HOME = 'C:\Program Files\Microsoft\jdk-17.0.20.101-hotspot'
& "$env:JAVA_HOME\bin\keytool.exe" -genkeypair `
    -v -keystore ..\bookhound-upload.jks `
    -alias upload `
    -keyalg RSA -keysize 2048 -validity 10000
```

## 2. Wire up signing

```powershell
Copy-Item keystore.properties.template keystore.properties
notepad keystore.properties   # fill in the two passwords + path
```

`keystore.properties` and `*.jks` are gitignored.

## 3. Bump version for each release

Edit `app/build.gradle.kts`:
```kotlin
versionCode = 2       // integer, monotonically increasing per upload
versionName = "1.1"   // user-facing string
```

## 4. Build the App Bundle (AAB — Play requires this, not APK)

```powershell
$env:JAVA_HOME = 'C:\Program Files\Microsoft\jdk-17.0.20.101-hotspot'
.\gradlew.bat :app:bundleRelease
```

Output: `app/build/outputs/bundle/release/app-release.aab`.

## 5. Smoke-test the release build on device

```powershell
.\gradlew.bat :app:installRelease
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" -s RFCY11JWE3J shell am start -n dev.bookhound.kmp/dev.bookhound.kmp.MainActivity
```

R8 is enabled — verify camera preview, OCR highlights, and vibrate work exactly like debug. If anything breaks, the ProGuard config in `app/proguard-rules.pro` is the first thing to check.

## 6. Play Console setup (one-time)

- Create the app in [Google Play Console](https://play.google.com/console).
- **App integrity → Play App Signing**: opt in, upload the upload certificate (Play holds the real signing key).
- **App content → Data safety**: declare "Camera used, not collected" (frames are processed on-device by ML Kit and never leave the phone).
- **App content → Permissions**: declare CAMERA (required for functionality).
- **App content → Privacy policy**: required because CAMERA is a runtime permission; host a short page saying no data is collected.
- **Content rating**: complete the questionnaire (this app is content-free — Everyone).
- **Target audience**: 13+ (or whatever fits).
- **Store listing**: app name, short + full description, feature graphic (1024×500), screenshots (min 2, phone), the adaptive icon we already ship.

## 7. Upload

Upload the `.aab` to Internal testing first, verify install via a tester's Play link, then promote to Closed → Open → Production.
