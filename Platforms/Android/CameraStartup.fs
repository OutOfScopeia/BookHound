// Camera startup function (Android-only)
// This is typically called from your MainActivity.
// * KEEP_ONLY_LATEST avoids frame pileup
// * Single-thread executor = predictable latency
// * No preview surface needed

open AndroidX.Camera.Lifecycle
open AndroidX.Camera.Core
open AndroidX.Camera.Camera2
open AndroidX.Core.Content
open Android.Content
open AndroidX.Lifecycle

let startCamera (context: Context) (lifecycleOwner: ILifecycleOwner) (onFrame: CameraFrame -> unit) =

    let cameraProviderFuture = ProcessCameraProvider.GetInstance(context)

    cameraProviderFuture.AddListener(
        Java.Lang.Runnable(fun () ->
            let cameraProvider = cameraProviderFuture.Get()

            let analysis =
                ImageAnalysis.Builder()
                    .SetBackpressureStrategy(
                        ImageAnalysis.StrategyKeepOnlyLatest)
                    .SetTargetFrameRate(
                        Range(10, 10)) // ≈10 FPS
                    .Build()

            let executor = Executors.NewSingleThreadExecutor()

            analysis.SetAnalyzer(
                executor,
                FrameAnalyzer(onFrame))

            let cameraSelector =
                CameraSelector.Builder()
                    .RequireLensFacing(CameraSelector.LensFacingBack)
                    .Build()

            cameraProvider.UnbindAll()

            cameraProvider.BindToLifecycle(
                lifecycleOwner,
                cameraSelector,
                analysis
            ) |> ignore
        ),
        ContextCompat.GetMainExecutor(context)
    )
