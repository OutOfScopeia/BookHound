namespace BookHoundApp.Camera.Android

open Microsoft.Maui.Controls
open BookHoundApp.Camera

type CameraPreviewView(cameraService: ICameraService) =
    inherit ContentView()

    let mutable started = false

    member this.EnsureStarted() =
        if not started then
            let preview =
                cameraService.StartPreview(fun frame ->
                    // frames arrive here
                    ()
                )

            // 🔥 THIS is the crucial line
            this.Content <- preview :?> View
            started <- true