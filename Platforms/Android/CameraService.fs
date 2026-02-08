namespace BookHoundApp.Camera.Android

open Android.App
open AndroidX.Camera.View
open AndroidX.Lifecycle
open BookHoundApp.Camera
open Microsoft.Maui
open Microsoft.Maui.ApplicationModel

open Microsoft.Maui.Controls
open Microsoft.Maui.Platform

type AndroidCameraService () =
    interface ICameraService with
        member _.StartPreview(onFrame) =
            let activity =
                Platform.CurrentActivity
                :?> MauiAppCompatActivity

            let previewView = new PreviewView(activity)

            CameraStartup.startCamera
                activity
                (activity :> ILifecycleOwner)
                previewView
                onFrame

            previewView :> obj