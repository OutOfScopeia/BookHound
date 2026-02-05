// An IAnalyzer that fires ~10 times/sec.
// If you forget image.Close() the pipeline WILL freeze.

namespace BookHoundApp.Camera.Android

open AndroidX.Camera.Core
open BookHoundApp.Camera
open Java.Lang

type FrameAnalyzer(onFrame : CameraFrame -> unit) =
    inherit Object()

    // Target ~10 FPS - every 100ms. The timestamp type on IImageProxy is "nanoseconds since boot" (a tick is 1ns).
    let targetIntervalNs = 100_000_000L
    let mutable lastTimestamp = 0L

    interface ImageAnalysis.IAnalyzer with
        member _.Analyze(image: IImageProxy) =
            try
                let ts = image.ImageInfo.Timestamp

                // Throttle to ~10 FPS
                if ts - lastTimestamp >= targetIntervalNs then
                    lastTimestamp <- ts

                    let planes = image.GetPlanes()

                    let yBuf, uBuf, vBuf =
                        planes.[0].Buffer,
                        planes.[1].Buffer,
                        planes.[2].Buffer

                    let y, u, v =
                        Array.zeroCreate<byte> (yBuf.Remaining()),
                        Array.zeroCreate<byte> (uBuf.Remaining()),
                        Array.zeroCreate<byte> (vBuf.Remaining())

                    yBuf.Get(y) |> ignore
                    uBuf.Get(u) |> ignore
                    vBuf.Get(v) |> ignore

                    onFrame {
                        Width       = image.Width
                        Height      = image.Height
                        Y           = y
                        U           = u
                        V           = v
                        TimestampNs = ts
                    }
            finally
                // ALWAYS close, even if frame is skipped
                image.Close()
