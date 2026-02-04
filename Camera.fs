namespace BookHoundApp.Camera

// chatgpt
type CameraFrame =
  { Width  : int
    Height : int
    Y      : byte[]
    U      : byte[]
    V      : byte[]
    TimestampNs : int64 }