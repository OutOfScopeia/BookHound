namespace BookHoundApp

open System.IO
open Fabulous
open Fabulous.Maui
open Fabulous.Maui.PathBuilders
open Microsoft.Maui
open Microsoft.Maui.Graphics
open Microsoft.Maui.Accessibility
open Microsoft.Maui.Primitives
open AndroidX.Camera.View
open Microsoft.Maui.Platform
open Microsoft.Maui.Controls
open Microsoft.Extensions.DependencyInjection
open BookHoundApp.Camera

open type Fabulous.Maui.View

module App =
    open Microsoft.Maui.Storage
    open Microsoft.Maui.Media
    open Microsoft.Maui.ApplicationModel
    
    type Model = {
        CameraService : ICameraService option
        HasCameraPermissions: bool
        TrackedString: string
        Photos: FileResult list
     }

    type Msg =
        | TargetStringChanged of string
        | TargetStringEntered
        | TargetStringRecognised
        | CapturePhotoClicked
        | PhotoCaptured of FileResult option
        | UpdateCameraPermStatusClicked
        | CameraPermissionStatusObtained of bool

    type CmdMsg =
        | UpdateCameraPermStatus
        | CapturePhoto
        // remove
        | SemanticAnnounce of string

    let cameraViewRef = ViewRef<obj>()

    let ensureCameraPermissionAsync() =
        task {
            let! status = Permissions.CheckStatusAsync<Permissions.Camera>()
            if status <> PermissionStatus.Granted then
                if Permissions.ShouldShowRationale<Permissions.Camera>() then
                    ()
                let! newStatus = Permissions.RequestAsync<Permissions.Camera>()
                return newStatus = PermissionStatus.Granted
            else
                return true
        }
    let capturePhoto () =
        task {
            let! granted = ensureCameraPermissionAsync()
            match granted with
            | true ->
                let! photo = MediaPicker.Default.CapturePhotoAsync()
                let! stream = photo.OpenReadAsync()
                let appDataDir = FileSystem.AppDataDirectory
                let filePath = Path.Combine(appDataDir, photo.FileName)

                use fileStream = File.Create filePath
                do! stream.CopyToAsync fileStream
                stream.Dispose()

                return PhotoCaptured (Some photo)

            | false -> return PhotoCaptured None

        }
        |> Cmd.ofTaskMsg
            
            
    let semanticAnnounce text =
        Cmd.ofSub(fun _ -> SemanticScreenReader.Announce(text))

    let updateCameraPermStatus () =
        task {
            let! hasPerm = ensureCameraPermissionAsync()
            return CameraPermissionStatusObtained hasPerm
        }
        |> Cmd.ofTaskMsg

    let mapCmd cmdMsg =
        match cmdMsg with
        | UpdateCameraPermStatus -> updateCameraPermStatus ()
        | CapturePhoto -> capturePhoto ()
        // remove
        | SemanticAnnounce text -> semanticAnnounce text

    let init (cameraService : ICameraService option) () =
        {
            CameraService = cameraService
            HasCameraPermissions = false
            Photos = []
            TrackedString = null
        }, [ UpdateCameraPermStatus ]

    let update msg model =
        match msg with
        
        | TargetStringChanged s -> model, [ SemanticAnnounce $"Clicked times" ]
        | TargetStringEntered    -> model, [ SemanticAnnounce $"Clicked times" ]
        | TargetStringRecognised -> model, [ SemanticAnnounce $"Clicked times" ]
        
        | CapturePhotoClicked -> model, [ CapturePhoto ]
        | PhotoCaptured (Some photo) -> { model with Photos = photo :: model.Photos }, []
        | PhotoCaptured None -> model, []
        | UpdateCameraPermStatusClicked -> model, [ UpdateCameraPermStatus ]
        | CameraPermissionStatusObtained status -> { model with HasCameraPermissions = status }, []


    // let cameraPreviewView () =
    // View.AndroidView(
    //     create = (fun context ->
    //         let previewView = new PreviewView(context)
    //         previewView
    //     ),
    //     update = (fun previewView ->
    //         let activity =
    //             previewView.Context :?> Android.App.Activity

    //         CameraStartup.startCamera
    //             previewView.Context
    //             activity
    //             previewView
    //             (fun frame ->
    //                 // 🔥 You get ~10–30 fps here
    //                 ()
    //             )
    //     )
    // let getCameraService () =
    //     Application.Current.Services.GetService<ICameraService>()

    let tryGetCameraService () =
        Application.Current
        |> Option.ofObj
        |> Option.bind (fun app ->
            app.Handler.MauiContext
            |> Option.ofObj
            |> Option.map (fun ctx -> ctx.Services.GetService<ICameraService>())
        )

    let view model =
        Application(
            ContentPage(
                    (VStack(spacing = 25.) {
                        // // 📸 CAMERA PREVIEW
                        // View.NativeView(cameraViewRef)
                        //     .height(300.)
                        //     .width(300.)
                        //     .backgroundColor(Colors.Black)

                        match tryGetCameraService() with
                        | Some camera ->
                            ViewElement.AndroidView(
                                create = fun _ ->
                                    camera.StartPreview(fun frame ->
                                        // you now have CameraFrame here 🎉
                                        ()
                                    ),
                                update = fun _ _ -> ()
                            )
                            .height(300.)
                        | None ->
                            Label("Camera not available")


                        Label($"Camera perm: {model.HasCameraPermissions}")
                            .semantics(SemanticHeadingLevel.Level1)
                            .font(size = 24.)
                            .textColor(if model.HasCameraPermissions then Colors.Green else Colors.Red)
                            .centerTextHorizontal()
                            .centerHorizontal()

                        Label("Enter the string to scan for")
                            .semantics(SemanticHeadingLevel.Level1)
                            .font(size = 24.)
                            .centerTextHorizontal()
                            .centerHorizontal()
                        
                        Label($"Photo set size: {model.Photos |> List.length}")
                            .semantics(SemanticHeadingLevel.Level1)
                            .font(size = 28.)
                            .centerTextHorizontal()
                            .centerHorizontal()

                        Entry("",TargetStringChanged)
                            .placeholder("Target string")
                            .semantics(hint = "Enter the string to scan for")
                            .font(size = 24.)
                            .centerHorizontal()

                        Button("Start Scan", TargetStringEntered)
                            .semantics(hint = "Confirm when done")
                            .centerHorizontal()

                        Button("Capture Photo", CapturePhotoClicked)
                            .semantics(hint = "Take a photo")
                            .centerHorizontal()    
                        
                        Button("Update Camera Perm Status", UpdateCameraPermStatusClicked)
                            .semantics(hint = "bool")
                            .centerHorizontal()
                    })
                        .padding(30., 0., 30., 0.)
                        .centerVertical()
            )
        )

    let program (cameraService: ICameraService option) = Program.statefulWithCmdMsg (init cameraService) update view mapCmd
