module GameTests
open System
open Xunit
open Core
open Game
open System.Diagnostics
open Message
open Newtonsoft.Json

let When = execThe CommandHandler.handleGames 

[<Xunit.Fact>]
let ``Given nothing happened yet, when we create a game, a game is created``  () = 
    let gameId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")    
    let playerId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bf")
    let date = DateTime.Parse("2014-12-31 09:34:12.456")
    let location = "Toulouse"
    
    Given  []
    |> When (gameId,0) (CreateGame( playerId,date,location))
    |> Then (Choice1Of2 (GameCreated(playerId,date,location)))


    
[<Xunit.Fact>]
let ``Given a game is created, when we create this game, an excption is raised``  () = 
    let gameId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")
    let playerId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bf")
    let date = DateTime.Parse("2014-12-31 09:34:12.456")
    let location = "Toulouse"

    Given  [ GameCreated( playerId,date,location) ]
    |> When (gameId,0) (CreateGame(playerId,date,location))
    |> Then (Choice2Of2 (["the game is already scheduled"]))



      