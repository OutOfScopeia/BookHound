BookHound

Mobile app that uses OCR to scan a book shelf for a desired book title.

On Windows, create a new Fabulous app. Then edit the .fsproj to update any net8.0 to net10.0 and yeet any target framework references to Tim Apple (net?.0-ios, net?.0-maccatalyst).

Run DOTNET WORKLOAD RESTORE

set envs for ANDROID_SDK\ROOT and JAVA_HOME to whatever convenient local folders

Run this build command, which will also Fetch any missing Android dependencies (might need to run it twice if the command is fetching the deps to empty folders):

dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=$env:ANDROID\_SDK\_ROOT -p:JavaSdkDirectory=$env:JAVA\_HOME -p:AcceptAndroidSdkLicenses=True -t:Run

dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=/apkshare/sdk -p:JavaSdkDirectory=/apkshare/jdk -p:AcceptAndroidSdkLicenses=True -t:Run BookHoundApp


--- FIGURE OUT HOW TO SHARE ANDROID SDK FOLDERS FROM CONTAINER3 FOR USE BY CONTAINER1

/apkshare/sdk/cmdline-tools/latest/bin/sdkmanager --update
yes | sdkmanager --licenses

apt-get update
apt-get install --only-upgrade openjdk-17-jdk







one-off tasks:
DOTNET WORKLOAD RESTORE

mkdir /apkshare/sdk

mkdir /apkshare/jdk

-cannot be root

dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=/apkshare/sdk -p:JavaSdkDirectory=/apkshare/jdk -p:AcceptAndroi
dSdkLicenses=True -t:Run BookHoundApp

inside the emulator (if all deps are fully updated - dockerfile has handled that)

dotnet workload restore BookHoundApp/BookHoundApp.sln
dotnet build -f net10.0-android -t:Run BookHoundApp/BookHoundApp.sln

dotnet build -f net10.0-android -p:OutputPath=/apkshare/build BookHoundApp/BookHoundApp.sln

android builds ignore 'OutputPath' arg
dotnet build -f net10.0-android BookHoundApp/BookHoundApp.sln

aapt:
$ANDROID_SDK_ROOT/build-tools/36.0.0


on emulator machine:
adb install -r /apkshare/build/com.cheekbytes.bookhound-Signed.apk
adb install -r /apkshare/build/com.companyname.BookHoundApp-Signed.apk
adb install -r /apkshare/build2/com.companyname.BookHoundApp-Signed.apk

dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=$ANDROID_SDK\_ROOT -p:JavaSdkDirectory=$JAVA\_HOME -p:AcceptAndroidSdkLicenses=True -t:Run

dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=/opt/android -p:JavaSdkDirectory=/usr/lib/jvm/java-17-openjdk-amd64 -p:AcceptAndroidSdkLicenses=True -t:Run

sdkmanager --update
sdkmanager "system-images;android-36;google\_apis;x86\_64"
avdmanager create avd --name fab-avd --package "system-images;android-36;google\_apis;x86\_64" --device pixel\_9 --force
emulator -avd fab-avd -no-snapshot -wipe-data -no-audio -no-window -no-accel -port 5554

sdkmanager "system-images;android-36;default;arm64-v8a"
avdmanager create avd --name fab-avd --package "system-images;android-36;default;arm64-v8a" --device pixel\_9 --force
emulator -avd fab-avd -no-snapshot -wipe-data -no-audio -no-window -no-accel -port 5554



WIRELESS DEBUGGING

pair over cable first
go to android menu / debugging to enable wireless debugging
note the ip/port/pairing code
then disconnect cable and reconnect over wifi:

adb connect 192.168.1.114:41033

"adb devices" should show:
192.168.1.114:35759     device
on top of something like
adb-RFCY11JWE3J-rwhGze._adb-tls-connect._tcp    device

to keep the screen on while debugging (otherwise it disconnects and comes up wiht a random new port next time, requiring manual reconnect)
adb -s 192.168.1.114:35759 shell svc power stayon true

inside container:
adb connect host.docker.internal:35759



Delete VS Code server caches inside the container (if it still starts)
Inside the container:
rm -rf ~/.vscode-server*
rm -rf ~/.vscode-remote
