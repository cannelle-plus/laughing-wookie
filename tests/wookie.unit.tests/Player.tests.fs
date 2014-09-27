module PlayerTests
open System
open Xunit
open Core
open Player
open System.Diagnostics
open Message
open Newtonsoft.Json

let When = execThe CommandHandler.handlePlayers
let metadata = { UserId="sdfs";UserName="sddsd";CorrelationId = Guid.NewGuid() }

[<Xunit.Fact>]
let ``Given nothing happened yet, when a player join the community, the community is joined by this player``  () = 
    let playerId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")    
    let username = "John"
    
    Given []
    |> When (playerId,0,metadata) (Join( username))
    |> Then (  Choice1Of2 ( Joined(username) ) )

[<Xunit.Fact>]
let ``Given nothing a player has already joined the community, when a player join the community, this command does not succeed``  () = 
    let playerId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")    
    let username = "John"
    
    Given [Joined(username)]
    |> When (playerId,0,metadata) (Join( username))
    |> Then (  Choice2Of2 ( ["player has already joined the community"] ) )

[<Xunit.Fact>]
let ``Given having joined in the commuity, when a player schedule a game, a game is scheduled``  () = 
    let playerId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")    
    let gameId = System.Guid.NewGuid()
    let date = DateTime.Now.AddMonths(1)
    let location = "Toulouse"
    let username = "John"
    
    Given [Joined(username)]
    |> When (playerId,0,metadata) (ScheduleGame( gameId,date,location))
    |> Then (Choice1Of2 ( GameScheduled( gameId,date,location) ))

[<Xunit.Fact>]
let ``Given having joined in the commuity, when a player join a game, a game is joined``  () = 
    let playerId = System.Guid.NewGuid()
    let gameId = System.Guid.NewGuid()
    let username = "John"
    
    Given [Joined(username)]
    |> When (playerId,0,metadata) (JoinGame( gameId))
    |> Then (Choice1Of2 ( GameJoined( gameId) ))


[<Xunit.Fact>]
let ``Given having joined in the commuity, when a player leave a game, a game is left``  () = 
    let playerId = System.Guid.NewGuid()
    let gameId = System.Guid.NewGuid()
    let username = "John"
    
    Given [Joined(username)]
    |> When (playerId,0,metadata) (LeaveGame( gameId))
    |> Then (Choice1Of2 ( GameLeft( gameId) ))

[<Xunit.Fact>]
let ``Given having joined in the community and scheduled a game, when a player cancel this game, this game is cancelled``  () = 
    let playerId = System.Guid.NewGuid()
    let gameId = System.Guid.NewGuid()
    let date = DateTime.Now.AddMonths(1)
    let location = "Toulouse"
    let username = "John"
    
    Given [Joined(username);GameScheduled( gameId,date,location)]
    |> When (playerId,0,metadata) (CancelGame( gameId))
    |> Then (Choice1Of2 ( GameCancelled( gameId) ))


[<Xunit.Fact>]
let ``Given having joined in the community and not scheduled a game, when a player cancel a game, this game is not cancelled``  () = 
    let playerId = System.Guid.NewGuid()
    let gameId = System.Guid.NewGuid()
    let date = DateTime.Now.AddMonths(1)
    let location = "Toulouse"
    let username = "John"
    
    Given [Joined(username);]
    |> When (playerId,0,metadata) (CancelGame( gameId))
    |> Then (Choice2Of2 ( ["this player cannot cancel this game"]))
