$env:JAVA_HOME = 'C:\Program Files\Microsoft\jdk-17.0.20.101-hotspot'
& "$env:JAVA_HOME\bin\keytool.exe" -genkeypair `
    -v -keystore ..\bookhound-upload.jks `
    -alias upload `
    -keyalg RSA -keysize 2048 -validity 10000