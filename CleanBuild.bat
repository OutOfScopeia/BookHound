dotnet clean
rmdir /s /q "bin"
rmdir /s /q "obj"
dotnet build -f net9.0-android -t:Run BookHoundApp.sln -v diag