// #warning "MauiProgram.fs is being compiled"
// namespace BookHoundApp

// open Fabulous
// open Fabulous.Maui
// open Microsoft.Maui.Hosting

// type MauiProgram =
//     static member CreateMauiApp() =
//         MauiApp
//             .CreateBuilder()
//             .UseFabulousApp(App.program)
//             .Build()

namespace BookHoundApp

open Fabulous
open Fabulous.Maui
open Microsoft.Maui.Hosting
open Microsoft.Extensions.DependencyInjection

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

// #if ANDROID
//         builder
//             .Services
//             .AddSingleton<ICameraService>(fun (_: System.IServiceProvider) ->
//                 let activity = Platform.CurrentActivity :?> Android.App.Activity
//                 activity |> AndroidCameraService :> ICameraService
//         )
//         |> ignore
// #endif
        builder.Build()