// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

#light
open System
open Reactors 
open Player
open EventStore.ClientAPI.SystemData

let host = "http://localhost:8081/"

let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
let endPointHttp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 2113)

let eventStoreConnection = EventStore.conn endPoint
let user = UserCredentials("admin","changeit")

[<EntryPoint>]
let main argv = 
    let server = WookieServer.start host eventStoreConnection user
    
    Console.WriteLine("Server started...")
    Console.ReadKey(true) |> ignore
    0
