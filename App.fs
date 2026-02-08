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
// open BookHoundApp.Camera.Android

open type Fabulous.Maui.View

module App =
    open Microsoft.Maui.Storage
    open Microsoft.Maui.Media
    open Microsoft.Maui.ApplicationModel
    
    type Model = {
        CameraService : ICameraService option
        CameraAttached : bool
        TrackedString: string
        Photos: FileResult list
     }

    type Msg =
        | TargetStringChanged of string
        | TargetStringEntered
        | TargetStringRecognised
        | CapturePhotoClicked
        | PhotoCaptured of FileResult option
        | CameraServiceObtained of ICameraService option

    type CmdMsg =
        | GetCameraService
        | CapturePhoto
        // remove
        | SemanticAnnounce of string

    // let cameraViewRef = ViewRef<obj>()
    let cameraHostRef = ViewRef<ContentView>()

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
    
    let tryGetCameraService () =
        task {
            let cs =
                Application.Current
                |> Option.ofObj
                |> Option.bind (fun app ->
                    app.Handler.MauiContext
                    |> Option.ofObj
                    |> Option.map (fun ctx -> ctx.Services.GetService<ICameraService>())
                    )
            return CameraServiceObtained cs
        }
        |> Cmd.ofTaskMsg
            
    let semanticAnnounce text =
        Cmd.ofSub(fun _ -> SemanticScreenReader.Announce(text))

    let mapCmd cmdMsg =
        match cmdMsg with
        | GetCameraService -> tryGetCameraService ()
        | CapturePhoto -> capturePhoto ()
        // remove
        | SemanticAnnounce text -> semanticAnnounce text

    let init () =
        {
            CameraService = None
            CameraAttached = false
            Photos = []
            TrackedString = null
        }, [ GetCameraService ]

    let update msg model =
        match msg with
        | TargetStringChanged s          -> { model with TrackedString = s}, []
        | TargetStringEntered                     -> model, [ GetCameraService ]
        | TargetStringRecognised                  -> model, []
        | CapturePhotoClicked                     -> model, [ CapturePhoto ]
        | PhotoCaptured (Some photo) -> { model with Photos = photo :: model.Photos }, []
        | PhotoCaptured None                      -> model, []

        | CameraServiceObtained (Some cs) ->
            match cameraHostRef.TryValue with
            | Some host ->
                let preview =
                    cs.StartPreview(fun frame ->
                        // camera frames arrive here
                        ()
                    )
                host.Content <- preview :?> View
            | _ -> ()
        
            { model with CameraService = Some cs; CameraAttached = true }, []

        | CameraServiceObtained None -> model, []

        // | CameraServiceObtained (Some cs) ->
        //     match cameraHostRef.TryValue with
        //     | Some host when host.Content = null ->
        //         let preview = cs.StartPreview(fun frame -> ())
        //         host.Content <- preview :?> View
        //     | _ -> ()

    let view model =

        Application(
            ContentPage(
                    (VStack(spacing = 25.) {
                        // View.NativeView
                        // View.PlatformView
                        // View.AndroidView

                        match model.CameraService with
                        | Some _ ->
                            ContentView(Label("Camera loading…").centerHorizontal().centerVertical())
                                .reference(cameraHostRef)
                                .height(300.)
                                .width(300.)
                                .centerHorizontal()

                        | None ->
                            Label("Camera service not available").centerHorizontal()

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
                    })
                        .padding(30., 0., 30., 0.)
                        .centerVertical()
            )
        )

    let program = Program.statefulWithCmdMsg init update view mapCmd
