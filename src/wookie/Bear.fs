module Bear

open System
open Core

type Commands =
    | SignIn of string*int
    | AddFriends of int list
    | ScheduleGame of Guid*DateTime*string
    | JoinGame of Guid
    | LeaveGame of Guid
    | CancelGame of Guid
    

type Events =
    | SignedIn of string*int
    | GameScheduled of Guid*DateTime*string
    | GameJoined of Guid
    | GameLeft of Guid
    | GameCancelled of Guid
    | FriendsAdded of int list
    
type State = {
    hasJoined : bool;
    gamesCreated : Guid list
}
with static member Initial = { hasJoined = false; gamesCreated = [] }

let apply state = function
    | SignedIn (username,avatarId) -> { state with hasJoined=true }
    | GameScheduled (gameId, gameDate, gameLocation) ->  { state with  gamesCreated = gameId::state.gamesCreated }
    | GameJoined (gameId) -> state
    | GameLeft (gameId) -> state
    | GameCancelled (gameId) -> { state with  gamesCreated = List.filter (fun g -> g <> gameId) state.gamesCreated }
    | FriendsAdded (friendsList) ->  state

open Validator 

module private Assert =
    let validSignIn Bear username = validator (fun p -> not p.hasJoined && not(String.IsNullOrEmpty(username)) ) ["Bear has already joined the community"] Bear 
    let validCancelGame Bear gameId = validator (fun (p,id) -> List.exists ((=)id) p.gamesCreated) ["this Bear cannot cancel this game"] (Bear,gameId)

let exec state = function
    | SignIn (username,avatarId)-> Assert.validSignIn state username <?>  SignedIn(username,avatarId) 
    | ScheduleGame (gameId,  gameDate, gameLocation) -> Choice1Of2  ( GameScheduled(gameId,   gameDate, gameLocation))
    | JoinGame gameId -> Choice1Of2(GameJoined(gameId))
    | LeaveGame gameId -> Choice1Of2(GameLeft(gameId))
    | CancelGame (gameId) -> Assert.validCancelGame state gameId <?> GameCancelled(gameId)
    | AddFriends (friendsList) -> Choice1Of2(FriendsAdded(friendsList))
    


    

        
