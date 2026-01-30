# install cmdline tools for Android SDK first

$env:ANDROID_SDK_ROOT = "C:\Sandbox\Android"

# Persist the environment variable for future sessions
[System.Environment]::SetEnvironmentVariable(
    "ANDROID_SDK_ROOT",
    $env:ANDROID_SDK_ROOT,
    [System.EnvironmentVariableTarget]::User
)

# Ensure sdkmanager is on PATH (adjust if needed)
$sdkManager = Join-Path $env:ANDROID_SDK_ROOT "cmdline-tools\bin\sdkmanager.bat"

if (-not (Test-Path $sdkManager)) {
    Write-Error "sdkmanager not found at: $sdkManager"
    exit 1
}

# Install components
& winget install Microsoft.OpenJDK.17
& $sdkManager --sdk_root=$env:ANDROID_SDK_ROOT "platform-tools"
& $sdkManager --sdk_root=$env:ANDROID_SDK_ROOT "platforms;android-34"
& $sdkManager --sdk_root=$env:ANDROID_SDK_ROOT "build-tools;34.0.0"
& $sdkManager --sdk_root=$env:ANDROID_SDK_ROOT "system-images;android-34;google_apis;x86_64"

# & $sdkManager --sdk_root=$env:ANDROID_SDK_ROOT "platforms;android-36"
# & $sdkManager --sdk_root=$env:ANDROID_SDK_ROOT "build-tools;36.0.0"
# & $sdkManager --sdk_root=$env:ANDROID_SDK_ROOT "system-images;android-36;google_apis;x86_64"
