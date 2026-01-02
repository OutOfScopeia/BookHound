BookHound

Mobile app that uses OCR to scan a book shelf for a desired book title.



Your containerising is no good here.

On Windows, create a new Fabulous app. Then edit the .fsproj to update any net8.0 to net10.0 and yeet any target framework references to Tim Apple (net?.0-ios, net?.0-maccatalyst).

Run DOTNET WORKLOAD RESTORE

set envs for ANDROID\_SDK\_ROOT and JAVA\_HOME to whatever convenient local folders

Run this build command, which will also Fetch any missing Android dependencies (might need to run it twice if the command is fetching the deps to empty folders):

dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=$env:ANDROID\_SDK\_ROOT -p:JavaSdkDirectory=$env:JAVA\_HOME -p:AcceptAndroidSdkLicenses=True -t:Run



inside the emulator:
dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=$ANDROID\_SDK\_ROOT -p:JavaSdkDirectory=$JAVA\_HOME -p:AcceptAndroidSdkLicenses=True -t:Run

dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=/opt/android -p:JavaSdkDirectory=/usr/lib/jvm/java-17-openjdk-amd64 -p:AcceptAndroidSdkLicenses=True -t:Run



sdkmanager --update
sdkmanager "system-images;android-36;google\_apis;x86\_64"
avdmanager create avd --name fab-avd --package "system-images;android-36;google\_apis;x86\_64" --device pixel\_9 --force
emulator -avd fab-avd -no-snapshot -wipe-data -no-audio -no-window -no-accel -port 5554

sdkmanager "system-images;android-36;default;arm64-v8a"
avdmanager create avd --name fab-avd --package "system-images;android-36;default;arm64-v8a" --device pixel\_9 --force
emulator -avd fab-avd -no-snapshot -wipe-data -no-audio -no-window -no-accel -port 5554















PS C:\\sandbox\\android-sdk\\platform-tools> $info = docker inspect vs-container | ConvertFrom-Json

PS C:\\sandbox\\android-sdk\\platform-tools> $info\[0].HostConfig.NetworkMode

bridge

PS C:\\sandbox\\android-sdk\\platform-tools> $info\[0].HostConfig.PortBindings



5555/tcp                    6080/tcp

--------                    --------

{@{HostIp=; HostPort=5557}} {@{HostIp=; HostPort=6082}}



PS C:\\sandbox\\android-sdk\\platform-tools> $info\[0].NetworkSettings.Ports



5555/tcp                                                        6080/tcp

--------                                                        --------

{@{HostIp=0.0.0.0; HostPort=5557}, @{HostIp=::; HostPort=5557}} {@{HostIp=0.0.0.0; HostPort=6082}, @{HostIp=::; HostPort=6082}}







PS C:\\sandbox> $info = docker inspect android-container | ConvertFrom-Json

PS C:\\sandbox> $info\[0].HostConfig.NetworkMode

bridge

PS C:\\sandbox> $info\[0].HostConfig.PortBindings



5555/tcp                    6080/tcp

--------                    --------

{@{HostIp=; HostPort=5556}} {@{HostIp=; HostPort=6081}}



PS C:\\sandbox> $info\[0].NetworkSettings.Ports



5555/tcp                                                        6080/tcp

--------                                                        --------

{@{HostIp=0.0.0.0; HostPort=5556}, @{HostIp=::; HostPort=5556}} {@{HostIp=0.0.0.0; HostPort=6081}, @{HostIp=::; HostPort=6081}}





















$a = docker inspect vs-container | ConvertFrom-Json

$b = docker inspect android-container | ConvertFrom-Json



\# Compare NetworkMode and PortBindings

\[PSCustomObject]@{

&nbsp; Field = 'NetworkMode'

&nbsp; VSCode = $a\[0].HostConfig.NetworkMode

&nbsp; DockerRun = $b\[0].HostConfig.NetworkMode

}

\[PSCustomObject]@{

&nbsp; Field = 'PortBindings'

&nbsp; VSCode = ($a\[0].HostConfig.PortBindings | ForEach-Object { "$($\_.Key)->$($\_.Value\[0].HostPort)" }) -join '; '

&nbsp; DockerRun = ($b\[0].HostConfig.PortBindings | ForEach-Object { "$($\_.Key)->$($\_.Value\[0].HostPort)" }) -join '; '

}













