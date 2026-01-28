# BookHound
### Mobile app that uses OCR to scan a book shelf for a desired book title.
## DevContainerised Environment
This is hybrid container image:
* Dotnet SDK 10 noble (base image)
* Android SDK tools + Android Emulator
* TigerVNC + NoVNC

TigerVNC for easy passwordless access. It is bound to localhost only, as the remote acccess over web is actually handled/proxied by NoVNC.
You can use this as a template if you want to use this dev setup with your own project. Just rip out the BookHound project (the whole BookHoundApp folder) and replace with your own. Obviously, fix all the references and names/labels in the files outside the folder (devcontainer.json, docker-compose.yml, etc).

1. Create a new [Fabulous F#](https://docs.fabulous.dev) app.
2. Replace the BookHoundApp folder with the folder of your new Fabulous project.
3. .fsproj: Update the app name and identifier.
4. .fsproj: Update any TFMs from net8.0 to net10.0.
5. .fsproj: Yeet any target framework references to Tim Apple (net?.0-ios, net?.0-maccatalyst).
6. .fsproj: Add this section - my app kept crashing inside the x86_64 emulator without it:
```XML
    <!-- Fixes crashes in android emulator. Remove the whole group if ever no longer needed (AndroidUseFastDeployment is deprecated already).  -->
  <PropertyGroup Condition="'$(TargetFramework)'=='net10.0-android'">
    <AndroidUseFastDeployment>false</AndroidUseFastDeployment>
    <AndroidFastDeploymentType>None</AndroidFastDeploymentType>
    <AndroidUseSharedRuntime>false</AndroidUseSharedRuntime>
    <EmbedAssembliesIntoApk>true</EmbedAssembliesIntoApk>
  </PropertyGroup>
```
6. .fsproj: Add this section if you want to use .NET Meteor VS extension for debugging:
```XML
  <!-- Added to make .NET Meteor work -->
  <ItemGroup>
    <Compile Remove="**\*.cs" />
  </ItemGroup>
```
7. In VS Code, open the folder in container.
8. There is a **PostCreate** command in the *devcontainer.json* that should do this (once, after the container is created). In case it didn't, from the in-container terminal, run:
```bash
dotnet workload restore BookHoundApp.sln
```
9. You should be able to build the app and deploy it to the emulator with:
```bash
dotnet build -f net10.0-android -t:Run BookHoundApp.sln
```
10. Open [localhost:6080/vnc.html](http://localhost:6080/vnc.html) to actually see what you're doing.

## Some more disorganised and possibly irrelevant notes
### WSL Version [Windows only]
Updating WSL to 2.6.3 fucked the whole thing, as it breaks the /dev/kvm virtualisation passthrough from the host to the container. You can get the .msi for 2.4.13 from the official channels to unfuck it.

### OutputPath arg for dotnet build
This won't work as the OutputPath is ignored for android builds. You can specify a folder, but that folder will be created inside the usual bin/Debug folder, never outside.
```bash
dotnet build -f net10.0-android -p:OutputPath=/apkshare/build BookHoundApp.sln
```

on emulator machine:
adb install -r /apkshare/build/com.cheekbytes.bookhound-Signed.apk

### Wireless Debugging
1. Pair over cable first
2. Go to android menu / debugging to enable wireless debugging
3. Note the ip/port/pairing code
4. Disconnect cable and reconnect over wifi:

```bash
adb connect 192.168.1.114:41033
```
"adb devices" should show:
192.168.1.114:35759     device
on top of something like
adb-RFCY11JWE3J-rwhGze._adb-tls-connect._tcp    device

to keep the screen on while debugging (otherwise it disconnects and comes up wiht a random new port next time, requiring manual reconnect)
```bash
adb -s 192.168.1.114:35759 shell svc power stayon true
```
inside the container:
```bash
adb connect host.docker.internal:35759
```
### Delete VS Code server caches inside the container (the crap you sometimes can't get rid of even with "rebuild without cache")
Inside the container:
```bash
rm -rf ~/.vscode-server*  
rm -rf ~/.vscode-remote  
```
