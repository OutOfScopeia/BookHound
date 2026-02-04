namespace BookHoundApp

open Android
open Android.App
open Android.OS
open Android.Content.PM
open Microsoft.Maui
open BookHoundApp.Camera
open BookHoundApp.Camera.Android

[<Activity(
        //    Name = "com.cheekbytes.bookhound.MainActivity",
           Theme = "@style/Maui.SplashTheme",
           MainLauncher = true,
           Label = "BookHoundApp",
           ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation ||| ConfigChanges.UiMode ||| ConfigChanges.ScreenLayout ||| ConfigChanges.SmallestScreenSize ||| ConfigChanges.Density))>]
type MainActivity() =
    inherit MauiAppCompatActivity()

    let requestId = 1001

    override this.OnCreate(bundle: Bundle) =
        base.OnCreate(bundle)

        if this.CheckSelfPermission(Manifest.Permission.Camera) <> Permission.Granted then
            this.RequestPermissions([| Manifest.Permission.Camera |], requestId)
