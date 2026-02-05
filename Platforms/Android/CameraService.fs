namespace BookHoundApp.Camera.Android

open Android.App
open AndroidX.Camera.View
open AndroidX.Lifecycle
open BookHoundApp.Camera
open Microsoft.Maui.ApplicationModel
open Microsoft.Maui

type AndroidCameraService (activity: Activity) =
    interface ICameraService with
        member _.StartPreview(onFrame) =

            let previewView = new PreviewView(activity)

            let lifecycleOwner =
                Platform.CurrentActivity
                :?> MauiAppCompatActivity
                :> ILifecycleOwner

            CameraStartup.startCamera activity lifecycleOwner previewView onFrame

            previewView :> obj
