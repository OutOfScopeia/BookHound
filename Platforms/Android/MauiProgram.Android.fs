namespace BookHoundApp

open BookHoundApp.Camera
open BookHoundApp.Camera.Android
open Microsoft.Extensions.DependencyInjection
open Microsoft.Maui.Hosting
open Microsoft.Maui.ApplicationModel

module AndroidCameraRegistration =

    let configure (builder: MauiAppBuilder) =
        builder.Services.AddSingleton<ICameraService, AndroidCameraService>()
        |> ignore
