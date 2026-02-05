namespace BookHoundApp.Camera.Android

module CameraStartup =
    open AndroidX.Camera.Lifecycle
    open AndroidX.Camera.Core
    open AndroidX.Camera.Camera2
    open AndroidX.Camera.View
    open AndroidX.Core.Content
    open Android.Content
    open AndroidX.Lifecycle
    open BookHoundApp.Camera
    open Java.Util.Concurrent
    open Java.Lang

    [<CompilerMessage("Compiling CameraStartup.fs", 42, IsError = false)>]
    let startCamera (context: Context) (lifecycleOwner: ILifecycleOwner) (previewView: PreviewView) (onFrame: CameraFrame -> unit) =

        let cameraProviderFuture = ProcessCameraProvider.GetInstance context

        cameraProviderFuture.AddListener(
            new Java.Lang.Runnable(fun () ->
                let cameraProvider = cameraProviderFuture.Get() :?> ProcessCameraProvider

                let preview =
                    Preview.Builder().Build()

                preview.SetSurfaceProvider previewView.SurfaceProvider

                let analysis =
                    ImageAnalysis.Builder()
                        .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest)
                        .Build()

                analysis.SetAnalyzer(
                    Executors.NewSingleThreadExecutor(),
                    new FrameAnalyzer(onFrame)
                )

                // let cameraSelector =
                //     CameraSelector.Builder()
                //         .RequireLensFacing(CameraSelector.LensFacingBack)
                //         .Build()

                let cameraSelector = CameraSelector.DefaultBackCamera

                cameraProvider.UnbindAll()

                cameraProvider.BindToLifecycle(
                    lifecycleOwner,
                    cameraSelector,
                    preview,
                    analysis
                ) |> ignore
            ),
            ContextCompat.GetMainExecutor context
        )




