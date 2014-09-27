// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

#light
open System
open Reactors 
open Player
open EventStore.ClientAPI.SystemData
open Newtonsoft.Json

open Message


let port = "8081"
let domain = "localhost" 
let host = "http://" + domain + ":" + port + "/"


let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
let endPointHttp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 2113)

let eventStoreConnection = EventStore.conn endPoint
let user = UserCredentials("admin","changeit")

[<EntryPoint>]
let main argv = 
    let server = WookieServer.start host eventStoreConnection user
//    let metaToken = { 
//        Key="TokenId"; 
//        Value=Guid.NewGuid().ToString() ;
//    }
//    let metaUser = { 
//        Key="UserId"; 
//        Value=Guid.NewGuid().ToString() ;
//    }
//    let metaUserName = { 
//        Key="UserName"; 
//        Value= "toto";
//    }
//
//    let msg:Command<_> = {
//        Id = Guid.NewGuid();
//        Version= 0;
//        MetaData = [ metaToken; metaUser; metaUserName]
//        PayLoad = Game.CreateGame("super nom", Guid.NewGuid(),DateTime.Now.AddDays(12.0),"toulouse",10)
//
//    }
//
//    let json = JsonConvert.SerializeObject(msg)
//    
//    Console.WriteLine(json)

    Console.WriteLine("Server started...")
    Console.WriteLine("listenning on " + host)
    
    Console.ReadKey(true) |> ignore
    0
