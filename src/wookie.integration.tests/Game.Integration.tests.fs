module GameIntegrationTests
//open System
//open Player
//
//
//
//[<Xunit.Fact>]
//let ``Given nothing happened yet, when some player schedule a game, a game is created``  () = 
//    let playerId = Guid.NewGuid()
//    let gameId = Guid.NewGuid()
//    let date = DateTime.Now.AddMonths(1)
//    let location = "Toulouse"
//
//    Given  []
//    |> When (playerId,0) (Player.ScheduleGame( gameId,date,location))
//    |> Then (Choice1of2(Game.GameCreated( playerId,date,location)))
//
