module Player

open System
open Core

type Commands =
    | Join of string
    | ScheduleGame of Guid*DateTime*string
    | JoinGame of Guid
    | LeaveGame of Guid
    | CancelGame of Guid

type Events =
    | Joined of string
    | GameScheduled of Guid*DateTime*string
    | GameJoined of Guid
    | GameLeft of Guid
    | GameCancelled of Guid
    
type State = {
    hasJoined : bool;
    gamesCreated : Guid list
}
with static member Initial = { hasJoined = false; gamesCreated = [] }

let apply state = function
    | Joined username -> { state with hasJoined=true }
    | GameScheduled (gameId, gameDate, gameLocation) ->  { state with  gamesCreated = gameId::state.gamesCreated }
    | GameJoined (gameId) -> state
    | GameLeft (gameId) -> state
    | GameCancelled (gameId) -> { state with  gamesCreated = List.filter (fun g -> g <> gameId) state.gamesCreated }

open Validator 

module private Assert =
    let validJoinCommunity player = validator (fun p -> not p.hasJoined  ) ["player has already joined the community"] player 
    let validCancelGame player gameId = validator (fun (p,id) -> List.exists ((=)id) p.gamesCreated) ["this player cannot cancel this game"] (player,gameId)

    
let exec state = function
    | Join (username)-> Assert.validJoinCommunity state <?>  Joined(username) 
    | ScheduleGame (gameId,  gameDate, gameLocation) -> Choice1Of2  ( GameScheduled(gameId,   gameDate, gameLocation))
    | JoinGame gameId -> Choice1Of2(GameJoined(gameId))
    | LeaveGame gameId -> Choice1Of2(GameLeft(gameId))
    | CancelGame (gameId) -> Assert.validCancelGame state gameId <?> GameCancelled(gameId)


    

        
