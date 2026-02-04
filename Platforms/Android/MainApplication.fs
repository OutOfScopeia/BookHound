namespace BookHoundApp

open Android.App
open Android.Runtime
open Microsoft.Maui
open System


[<Application>]
type MainApplication(handle: IntPtr, owningTransfer: JniHandleOwnership) =
    inherit MauiApplication(handle, owningTransfer)

    override _.CreateMauiApp() =
        MauiProgram.CreateMauiApp()