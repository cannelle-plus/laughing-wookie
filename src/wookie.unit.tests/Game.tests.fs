module GameTests
open System
open Xunit
open Core
open Game
open System.Diagnostics
open Message
open Newtonsoft.Json

let When = execThe CommandHandler.handleGames 
let metadata = { UserId="sdfs";UserName="sddsd";CorrelationId = Guid.NewGuid() }

[<Xunit.Fact>]
let ``Given nothing happened yet, when we create a game, a game is created``  () = 
    let gameId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")    
    let gameName= "tttttt"
    let ownerId = "88085239-6f0f-48c6-b73d-017333cb99bf"
    let date = DateTime.Parse("2014-12-31 09:34:12.456")
    let location = "Toulouse"
    let nbRequiredPlayers = 10
    
    Given  []
    |> When (gameId,0) ( CreateGame(gameName, ownerId,date,location, nbRequiredPlayers))
    |> Then (Choice1Of2 (GameCreated(gameName, ownerId,date,location,nbRequiredPlayers)))


    
[<Xunit.Fact>]
let ``Given a game is created, when we create this game, an excption is raised``  () = 
    let gameId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")
    let gameName= "tttttt"
    let ownerId = "88085239-6f0f-48c6-b73d-017333cb99bf"
    let date = DateTime.Parse("2014-12-31 09:34:12.456")
    let location = "Toulouse"
    let nbRequiredPlayers = 10

    Given  [ GameCreated(gameName, ownerId,date,location,nbRequiredPlayers) ]
    |> When (gameId,0) (CreateGame( gameName, ownerId,date,location,nbRequiredPlayers))
    |> Then (Choice2Of2 (["the game is already scheduled"]))



      