namespace BookHoundApp

open Fabulous
open Fabulous.Maui
open Microsoft.Maui.Hosting
open Microsoft.Extensions.DependencyInjection
open System
open BookHoundApp.Camera

type MauiProgram =
    static member CreateMauiApp() =
        
        let builder =
            MauiApp.CreateBuilder()
                .UseFabulousApp(App.program)
                // Fabulous template app remnant - yeet this
                .ConfigureFonts(fun fonts ->
                    fonts
                        .AddFont("OpenSans-Regular.ttf", "OpenSansRegular")
                        .AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold")
                    |> ignore)

        // This call only exists on Android builds
        // because the module only exists there
        AndroidCameraRegistration.configure builder

        builder.Build()

        // let createApp (services : IServiceProvider) =
        // let cameraService =
        //     services.GetService<ICameraService>()

        // Program.stateful
        //     (init cameraService)
        //     update
        //     view
