namespace BookHoundApp.WinUI

open System

module Program =
    [<EntryPoint; STAThread>]
    let main args =
        do FSharp.Maui.WinUICompat.Program.Main(args, typeof<BookHoundApp.WinUI.App>)
        0
