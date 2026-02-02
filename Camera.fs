namespace BookHoundApp.Camera

// chatgpt
type CameraFrame =
  { Width  : int
    Height : int
    Y      : byte[]
    U      : byte[]
    V      : byte[]
    TimestampNs : int64 }


// copilot gpt :D
// type CameraFrame =
//     { Width: int
//       Height: int
//       Data: byte[] } // Y plane (YUV_420_888)

// type ICameraService =
//     abstract member Start : (CameraFrame -> unit) -> unit
//     abstract member Stop : unit -> unit


