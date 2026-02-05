namespace BookHoundApp.Camera.Android

open Android.Content
open AndroidX.Camera.View
open AndroidX.Lifecycle
open BookHoundApp.Camera

type AndroidCameraService (context: Context, lifecycleOwner: ILifecycleOwner) =

    interface ICameraService with

        [<CompilerMessage("Compiling CameraServices.fs", 42, IsError = false)>]
        member _.StartPreview(onFrame) =

            let previewView = new PreviewView(context)

            CameraStartup.startCamera context lifecycleOwner previewView onFrame

            previewView :> obj


// open AndroidX.Camera.View
// open BookHoundApp.Camera
// open Android.App

// type AndroidCameraService(activity: Activity) =
//     interface ICameraService with
        
//         [<CompilerMessage("Compiling CameraServices.fs", 42, IsError = false)>]
//         member _.StartPreview(onFrame) =
//             let previewView = new PreviewView(activity)

//             CameraStartup.startCamera activity (activity :?> AndroidX.Lifecycle.ILifecycleOwner) previewView onFrame

//             previewView :> obj
