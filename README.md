# BookHound
Mobile app that uses OCR to scan a book shelf for a desired book title.


Your containerising is no good here.

On Windows, create a new Fabulous app. Then edit the .fsproj to update any net8.0 to net10.0 and yeet any target framework references to Tim Apple (net?.0-ios, net?.0-maccatalyst).

Run DOTNET WORKLOAD RESTORE

Run this build command, which will also Fetch any missing Android dependencies (it won't set the sdk folders into PATH though):
dotnet build -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=z:\android-sdk -p:JavaSdkDirectory=z:\jdk -p:AcceptAndroidSdkLicenses=True -t:Run