namespace BookHoundApp

open System.IO
open Fabulous
open Fabulous.Maui
open Fabulous.Maui.PathBuilders
open Microsoft.Maui
open Microsoft.Maui.Graphics
open Microsoft.Maui.Accessibility
open Microsoft.Maui.Primitives

open type Fabulous.Maui.View

module App =
    open Microsoft.Maui.Storage
    open Microsoft.Maui.Media
    open Microsoft.Maui.ApplicationModel
    type Model = {
        TrackedString: string
        Photos: FileResult list
     }

    type Msg =
        | TargetStringChanged of string
        | TargetStringEntered
        | TargetStringRecognised
        | CapturePhotoClicked
        | PhotoCaptured of FileResult option

    type CmdMsg =
        | CapturePhoto
        // remove
        | SemanticAnnounce of string

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

    let capturePhoto =
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

    let mapCmd cmdMsg =
        match cmdMsg with
        | CapturePhoto -> capturePhoto
        // remove
        | SemanticAnnounce text -> semanticAnnounce text

    let init () = { Photos = []; TrackedString = null }, []

    let update msg model =
        match msg with
        
        | TargetStringChanged s -> model, [ SemanticAnnounce $"Clicked times" ]
        | TargetStringEntered    -> model, [ SemanticAnnounce $"Clicked times" ]
        | TargetStringRecognised -> model, [ SemanticAnnounce $"Clicked times" ]
        
        | CapturePhotoClicked -> model, [ CapturePhoto ]
        | PhotoCaptured (Some photo) -> { model with Photos = photo :: model.Photos }, []
        | PhotoCaptured None -> model, []

    let view model =
        Application(
            ContentPage(
                    (VStack(spacing = 25.) {
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
