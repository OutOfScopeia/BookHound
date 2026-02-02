// An IAnalyzer that fires ~10 times/sec.
// If you forget image.Close() the pipeline WILL freeze.

namespace BookHoundApp.Camera.Android

open AndroidX.Camera.Core
open BookHoundApp.Camera
open Java.Util.Concurrent

type FrameAnalyzer(onFrame : CameraFrame -> unit) =
    interface ImageAnalysis.IAnalyzer with
        member _.Analyze(image: ImageProxy) =
            try
                let planes = image.Planes

                let yBuf = planes.[0].Buffer
                let uBuf = planes.[1].Buffer
                let vBuf = planes.[2].Buffer

                let y = Array.zeroCreate<byte> yBuf.Remaining()
                let u = Array.zeroCreate<byte> uBuf.Remaining()
                let v = Array.zeroCreate<byte> vBuf.Remaining()

                yBuf.Get(y)
                uBuf.Get(u)
                vBuf.Get(v)

                onFrame {
                    Width = image.Width
                    Height = image.Height
                    Y = y
                    U = u
                    V = v
                    TimestampNs = image.ImageInfo.Timestamp
                }
            finally
                image.Close() // CRITICAL: avoid backpressure stall
