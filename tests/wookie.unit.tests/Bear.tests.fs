module BearTests
open System
open Xunit
open Core
open Bear
open System.Diagnostics
open Message
open Newtonsoft.Json

let When = execThe CommandHandler.handleBears
let metadata = { UserId="sdfs";UserName="sddsd";CorrelationId = Guid.NewGuid() }

[<Xunit.Fact>]
let ``Given nothing happened yet, when a Bear signs in, the community is joined by this Bear``  () = 
    let BearId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")    
    let username = "John"
    let avatarId = 3 
    
    Given []
    |> When (BearId,0,metadata) (SignIn( username,avatarId))
    |> Then (  Choice1Of2 ( SignedIn(username,avatarId) ) )

[<Xunit.Fact>]
let ``Given a Bear has already signed in, when a Bear signs in, this command does not succeed``  () = 
    let BearId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")    
    let username = "John"
    let avatarId = 3 
    
    Given [SignedIn(username,avatarId)]
    |> When (BearId,0,metadata) (SignIn( username, avatarId))
    |> Then (  Choice2Of2 ( ["Bear has already joined the community"] ) )

[<Xunit.Fact>]
let ``Given having signed in, when a Bear schedule a game, a game is scheduled``  () = 
    let BearId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")    
    let gameId = System.Guid.NewGuid()
    let date = DateTime.Now.AddMonths(1)
    let location = "Toulouse"
    let username = "John"
    let avatarId = 3 
    
    Given [SignedIn(username,avatarId)]
    |> When (BearId,0,metadata) (ScheduleGame( gameId,date,location))
    |> Then (Choice1Of2 ( GameScheduled( gameId,date,location) ))

[<Xunit.Fact>]
let ``Given having signed in, when a Bear join a game, a game is joined``  () = 
    let BearId = System.Guid.NewGuid()
    let gameId = System.Guid.NewGuid()
    let username = "John"
    let avatarId = 3 
    
    Given [SignedIn(username,avatarId)]
    |> When (BearId,0,metadata) (JoinGame( gameId))
    |> Then (Choice1Of2 ( GameJoined( gameId) ))


[<Xunit.Fact>]
let ``Given having signed in, when a Bear leave a game, a game is left``  () = 
    let BearId = System.Guid.NewGuid()
    let gameId = System.Guid.NewGuid()
    let username = "John"
    let avatarId = 3 
    
    Given [SignedIn(username,avatarId)]
    |> When (BearId,0,metadata) (LeaveGame( gameId))
    |> Then (Choice1Of2 ( GameLeft( gameId) ))

[<Xunit.Fact>]
let ``Given having signed in and scheduled a game, when a Bear cancel this game, this game is cancelled``  () = 
    let BearId = System.Guid.NewGuid()
    let gameId = System.Guid.NewGuid()
    let date = DateTime.Now.AddMonths(1)
    let location = "Toulouse"
    let username = "John"
    let avatarId = 3 
    
    Given [SignedIn(username,avatarId);GameScheduled( gameId,date,location)]
    |> When (BearId,0,metadata) (CancelGame( gameId))
    |> Then (Choice1Of2 ( GameCancelled( gameId) ))


[<Xunit.Fact>]
let ``Given having signed in and not scheduled a game, when a Bear cancel a game, this game is not cancelled``  () = 
    let BearId = System.Guid.NewGuid()
    let gameId = System.Guid.NewGuid()
    let date = DateTime.Now.AddMonths(1)
    let location = "Toulouse" 
    let username = "John"
    let avatarId = 3 
    
    Given [SignedIn(username,avatarId);]
    |> When (BearId,0,metadata) (CancelGame( gameId))
    |> Then (Choice2Of2 ( ["this Bear cannot cancel this game"]))


[<Xunit.Fact>]
let ``Given a Bear has signed in, when a Bear add friends, new friends are added``  () = 
    let BearId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")    
    let username = "John"
    let avatarId = 3 

    let friends = [1;2;3]
    
    Given [SignedIn(username,avatarId)]
    |> When (BearId,1,metadata) (AddFriends( friends))
    |> Then ( Choice1Of2( FriendsAdded(friends)  ))