module IntegrationTests

open System
open EventStore.ClientAPI.SystemData

let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
let endPointHttp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 2113)

let conn = EventStore.conn endPoint

let user = UserCredentials("admin","changeit")

let handleCommand' =
    Aggregate.makeHandler 
        { initial = Game.State.Initial; apply = Game.apply; exec = Game.exec }
        (EventStore.makeRepository conn "Game" Serialization.serializer user )

let handleCommand (id,v) c = handleCommand' (id,v) c 

let id = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99ba")

//[<Xunit.Fact>]
//let initProjections() = 
//    let logger = new EventStore.ClientAPI.Common.Log.ConsoleLogger();
//    let pm = new EventStore.ClientAPI.ProjectionsManager(logger, endPointHttp)
//    let file p = System.IO.File.ReadAllText(@"..\..\" + p)
//    pm.CreateContinuous("FlatReadModelProjection", file "FlatReadModelProjection.js", user)
//    ()

let gameId = Guid.NewGuid()

[<Xunit.Fact>]
let createGame() =     
    let version = 0
    Game.CreateGame(gameId,DateTime.Now,DateTime.Now.AddHours(26.0),"Toulouse") |> handleCommand (gameId,version)    

[<Xunit.Fact>]
let joinGame() =     
    let version = 1
    Game.JoinGame(gameId) |> handleCommand (gameId,version)    

[<Xunit.Fact>]
let joinGameSecondPlayer() =     
    let version = 2
    Game.JoinGame(gameId) |> handleCommand (gameId,version)    

[<Xunit.Fact>]
let joinGameThirdPlayer() =     
    let version = 3
    Game.JoinGame(gameId) |> handleCommand (gameId,version)    

[<Xunit.Fact>]
let abandonGame() =     
    let version = 4
    Game.AbandonGame(gameId) |> handleCommand (gameId,version)    

