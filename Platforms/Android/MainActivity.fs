namespace BookHoundApp

open Android
open Android.App
open Android.OS
open Android.Content.PM
open Microsoft.Maui
open BookHoundApp.Camera
open BookHoundApp.Camera.Android

[<Activity(
           Name = "com.cheekbytes.bookhound.MainActivity",
           Theme = "@style/Maui.SplashTheme",
           Exported = true,
           MainLauncher = true,
           Label = "BookHoundApp",
           ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation ||| ConfigChanges.UiMode ||| ConfigChanges.ScreenLayout ||| ConfigChanges.SmallestScreenSize ||| ConfigChanges.Density))>]

type MainActivity() =
    inherit MauiAppCompatActivity()

    let cameraRequestCode = 1001

    override this.OnCreate(bundle: Bundle) =
        base.OnCreate(bundle)

        if this.CheckSelfPermission(Manifest.Permission.Camera)
           <> Permission.Granted then

            this.RequestPermissions(
                [| Manifest.Permission.Camera |],
                cameraRequestCode
            )

    override this.OnRequestPermissionsResult (requestCode: int, permissions: string[], grantResults: Permission[]) =

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults)

        if requestCode = cameraRequestCode then
            if grantResults.Length > 0 &&
               grantResults.[0] = Permission.Granted then
                // ✅ Permission granted
                ()
            else
                // ❌ Permission denied
                ()
