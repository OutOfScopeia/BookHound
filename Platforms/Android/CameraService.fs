namespace BookHoundApp.Camera.Android

open Android.App
open AndroidX.Camera.View
open AndroidX.Lifecycle
open BookHoundApp.Camera
open Microsoft.Maui
open Microsoft.Maui.ApplicationModel

open Microsoft.Maui.Controls
open Microsoft.Maui.Platform

// type AndroidCameraService(activity: Activity) =

//     interface ICameraService with
//         member _.StartPreview(onFrame) =

//             let previewView = new PreviewView(activity)

//             let lifecycleOwner =
//                 Platform.CurrentActivity
//                 :?> MauiAppCompatActivity
//                 :> ILifecycleOwner

//             CameraStartup.startCamera
//                 activity
//                 lifecycleOwner
//                 previewView
//                 onFrame

//             // 🔑 Convert Android View → MAUI View
//             previewView.ToPlatformView()


// type AndroidCameraService () =

//     interface ICameraService with
//         member _.StartPreview(onFrame) =

//             let activity =
//                 Platform.CurrentActivity
//                 :?> MauiAppCompatActivity

//             let previewView = new PreviewView(activity)

//             CameraStartup.startCamera
//                 activity
//                 (activity :> ILifecycleOwner)
//                 previewView
//                 onFrame

//             previewView :> obj


type AndroidCameraService(activity: Activity) =
    interface ICameraService with
        member _.AttachPreview(host: ContentView, onFrame) =

            let previewView = new PreviewView(activity)

            let lifecycleOwner =
                Platform.CurrentActivity
                :?> MauiAppCompatActivity
                :> ILifecycleOwner

            CameraStartup.startCamera activity lifecycleOwner previewView onFrame

            // ⚠️ MAUI bridge layer
            host.Loaded.Add(fun _ ->
                let vg = host.Handler.PlatformView :?> Android.Views.ViewGroup
                vg.AddView(previewView)
            )
