module IntegrationTests
//
//open System
//open EventStore.ClientAPI.SystemData
//open Message
//
//let host = "http://localhost:8081/"
//let siteRoot = @"C:\Mes documents\Visual Studio 2008\Projects\Drive\SiteWeb\"
//let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
//let endPointHttp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 2113)
//
//let conn = EventStore.conn endPoint
//let user = UserCredentials("admin","changeit")
//
//
//let handleCommand' =
//    Aggregate.makeHandler 
//        { initial = Game.State.Initial; apply = Game.apply; exec = Game.exec }
//        (EventStore.makeRepository conn  Serialization.serializer user "Game")
//
//let handleCommand (id,v) c = handleCommand' (id,v) c 
//
//let id = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99ba")
//
//[<Xunit.Fact>]
//let initProjections() = 
//    let logger = new EventStore.ClientAPI.Common.Log.ConsoleLogger();
//    let pm = new EventStore.ClientAPI.ProjectionsManager(logger, endPointHttp)
//    let file p = System.IO.File.ReadAllText(@"..\..\" + p)
//    pm.CreateContinuous("FlatReadModelProjection", file "FlatReadModelProjection.js", user)
//    ()
//
//let gameId = Guid.NewGuid()
////
//
//[<Xunit.Fact>]
//let ScheduleGame() =      
//    let server = WookieServer.start    
//    Console.ReadKey(true) |> ignore
////    let version = 0
////    Game.ScheduleGame(Guid.NewGuid(), DateTime.Now.AddHours(26.0),"Toulouse") 
////    |> handleCommand (gameId,version) 
//
//
////[<Xunit.Fact>]
////let joinGame() =     
////    let version = 1
////    Game.JoinGame(gameId,"totoDu31") |> handleCommand (gameId,version)    
////
////[<Xunit.Fact>]
////let joinGameSecondBear() =     
////    let version = 2
////    Game.JoinGame(gameId,"tataDu31") |> handleCommand (gameId,version)    
////
////[<Xunit.Fact>]
////let joinGameThirdBear() =     
////    let version = 3
////    Game.JoinGame(gameId,"titiDu31") |> handleCommand (gameId,version)    
////
////[<Xunit.Fact>]
////let abandonGame() =     
////    let version = 4
////    Game.AbandonGame(gameId,"totoDu31") |> handleCommand (gameId,version)    
////

//-----------------------------------------------------------------------
//STARTING THE EVENSTORE WIHTIN TEST
//-----------------------------------------------------------------------

//module GameTests
//open System
//open Xunit
//open Core
//open Game
//open System.Diagnostics
//open Message
//open Newtonsoft.Json
//
//let  eventStorePath = @"C:\Program Files\EventStore-NET-v3.0.0rc2\EventStore.SingleNode.exe"
//let paramseventStore = " --run-projections=all --mem-db"
//
//let makeJson (id,version) cmd =
//    let msg =  
//        {
//            Command.Id =  id
//            Version = 0;
//            CorrelationId = Guid.NewGuid();
//            TokenId = Guid.NewGuid();
//            PayLoad = cmd
//        }
//    JsonConvert.SerializeObject(msg)
//
//
//
//type ``Given a LightBulb that has had its state set to true`` ()=
//
//    let psi =new ProcessStartInfo(UseShellExecute = false, CreateNoWindow = true, FileName = eventStorePath,   Arguments = paramseventStore)
//    let myProcess = Process.Start(psi) 
//
//
//    [<Xunit.Fact>]
//    let ``Given nothing happened yet, when we create a game, a game is scheduled``  () = 
//        let version = 0
//        Game.ScheduleGame(Guid.NewGuid(), DateTime.Now.AddHours(26.0),"Toulouse") 
//        |> makeJson (gameId,version) 
//
//    interface System.ID  isposable with 
//        member this.Dispose() = myProcess.Kill()
 