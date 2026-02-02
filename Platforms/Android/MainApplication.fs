namespace BookHoundApp

open Android.App
open Android.Runtime
open Microsoft.Maui


[<Application(Name = "com.cheekbytes.bookhound.MainApplication")>]
type MainApplication(handle: IntPtr, owningTransfer: JniHandleOwnership) =
    inherit MauiApplication(handle, owningTransfer)

    override _.CreateMauiApp() =
        MauiProgram.CreateMauiApp()


// namespace BookHoundApp

// open Android.App
// open Microsoft.Maui

// [<Application>]
// type MainApplication(handle, ownership) =
//     inherit MauiApplication(handle, ownership)

//     override _.CreateMauiApp() = MauiProgram.CreateMauiApp()
