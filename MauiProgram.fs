namespace BookHoundApp

open Fabulous
open Fabulous.Maui
open Microsoft.Maui.Hosting
open Microsoft.Extensions.DependencyInjection

// new

// type MauiProgram =
//     static member CreateMauiApp() =
//         let builder = MauiApp.CreateBuilder()

//         builder
//             .UseFabulousApp(App.program)
//             .Services
//                 .AddSingleton<ICameraService, CameraService>()
//             |> ignore

//         builder.Build()

// old
type MauiProgram =
    static member CreateMauiApp() =
        MauiApp
            .CreateBuilder()
            .UseFabulousApp(App.program)
            .ConfigureFonts(fun fonts ->
                fonts
                    .AddFont("OpenSans-Regular.ttf", "OpenSansRegular")
                    .AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold")
                |> ignore)
            .Build()

