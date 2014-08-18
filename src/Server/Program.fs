// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

#light
open System
    

[<EntryPoint>]
let main argv = 
    let server = WookieServer.start
    Console.ReadKey(true) |> ignore
    0
