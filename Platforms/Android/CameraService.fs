namespace BookHoundApp.Camera.Android

open AndroidX.Camera.View
open BookHoundApp.Camera
open Android.App

type AndroidCameraService(activity: Activity) =
    interface ICameraService with
        member _.StartPreview(onFrame) =
            let previewView = new PreviewView(activity)

            CameraStartup.startCamera activity (activity :?> AndroidX.Lifecycle.ILifecycleOwner) previewView onFrame

            previewView :> obj