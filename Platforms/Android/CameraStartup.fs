namespace BookHoundApp.Camera.Android

module CameraStartup =
    open AndroidX.Camera.Lifecycle
    open AndroidX.Camera.Core
    open AndroidX.Camera.Camera2
    open AndroidX.Core.Content
    open Android.Content
    open AndroidX.Lifecycle
    open BookHoundApp.Camera
    open Java.Util.Concurrent

    let startCamera (context: Context) (lifecycleOwner: ILifecycleOwner) (onFrame: CameraFrame -> unit) =

        let cameraProviderFuture = ProcessCameraProvider.GetInstance(context)

        cameraProviderFuture.AddListener(
            new Java.Lang.Runnable(fun () ->
                let cameraProvider = cameraProviderFuture.Get() :?> ProcessCameraProvider

                let analysis =
                    ImageAnalysis.Builder()
                        .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest)
                        .Build()

                let executor = Executors.NewSingleThreadExecutor()

                analysis.SetAnalyzer(
                    executor,
                    new FrameAnalyzer(onFrame))

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
