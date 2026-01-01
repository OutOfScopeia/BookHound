BookHound

Mobile app that uses OCR to scan a book shelf for a desired book title.



Your containerising is no good here.

On Windows, create a new Fabulous app. Then edit the .fsproj to update any net8.0 to net10.0 and yeet any target framework references to Tim Apple (net?.0-ios, net?.0-maccatalyst).

Run DOTNET WORKLOAD RESTORE

set envs for ANDROID_SDK_ROOT and JAVA_HOME to whatever convenient local folders

Run this build command, which will also Fetch any missing Android dependencies (might need to run it twice if the command is fetching the deps to empty folders):

dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=$env:ANDROID_SDK_ROOT -p:JavaSdkDirectory=$env:JAVA_HOME -p:AcceptAndroidSdkLicenses=True -t:Run


inside the emulator:
dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=$ANDROID_SDK_ROOT -p:JavaSdkDirectory=$JAVA_HOME -p:AcceptAndroidSdkLicenses=True -t:Run

dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=/opt/android -p:JavaSdkDirectory=/usr/lib/jvm/java-17-openjdk-amd64 -p:AcceptAndroidSdkLicenses=True -t:Run


sdkmanager --update
sdkmanager "system-images;android-36;google_apis;x86_64"
avdmanager create avd --name fab-avd --package "system-images;android-36;google_apis;x86_64" --device pixel_9 --force
emulator -avd fab-avd -no-snapshot -wipe-data -no-audio -no-window -no-accel -port 5554

sdkmanager "system-images;android-36;default;arm64-v8a"
avdmanager create avd --name fab-avd --package "system-images;android-36;default;arm64-v8a" --device pixel_9 --force
emulator -avd fab-avd -no-snapshot -wipe-data -no-audio -no-window -no-accel -port 5554