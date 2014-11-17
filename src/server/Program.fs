// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

#light
open System
open Reactors 
open Bear
open EventStore.ClientAPI.SystemData
open Newtonsoft.Json
open System.Configuration

open Message


let port = ConfigurationManager.AppSettings.["Port"]
let domain = ConfigurationManager.AppSettings.["Host"]

let eventStoreHost =ConfigurationManager.AppSettings.["EventStoreHost"]
let eventStoreTCPPort = System.Int32.Parse( ConfigurationManager.AppSettings.["EventStoreTCPPort"])
let eventStoreHttpPort = System.Int32.Parse(ConfigurationManager.AppSettings.["EventStoreHttpPort"])

let host = "http://" + domain + ":" + port + "/"

let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(eventStoreHost), eventStoreTCPPort)
let endPointHttp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(eventStoreHost), eventStoreHttpPort)

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
//        PayLoad = Game.ScheduleGame("super nom", Guid.NewGuid(),DateTime.Now.AddDays(12.0),"toulouse",10)
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
