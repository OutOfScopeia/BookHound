namespace BookHoundApp

open Android.App
open Android.Runtime
open BookHoundApp.Camera
open BookHoundApp.Camera.Android
open Microsoft.Extensions.DependencyInjection
open Microsoft.Maui
open Microsoft.Maui.Hosting
open System

[<Application>]
type MainApplication(handle: IntPtr, owningTransfer: JniHandleOwnership) =
    inherit MauiApplication(handle, owningTransfer)

    override _.CreateMauiApp() =
            MauiProgram.CreateMauiApp()