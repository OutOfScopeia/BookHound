namespace BookHoundApp.Camera

open Microsoft.Maui.Controls

// chatgpt
type CameraFrame =
  { Width  : int
    Height : int
    Y      : byte[]
    U      : byte[]
    V      : byte[]
    TimestampNs : int64 }

type ICameraService =
    // abstract StartPreview : onFrame: (CameraFrame -> unit) -> obj
    abstract AttachPreview : host: ContentView * onFrame: (CameraFrame -> unit) -> unit
