module Game

open System
open Core

type Commands =
    | CreateGame of Guid*string*DateTime*string
    | JoinGame of Guid*string
    | AbandonGame of Guid*string

type Events =
    | GameCreated of Guid*string*DateTime*string
    | GameJoined of Guid*string
    | GameAbandonned of Guid*string

type State = {
    nbPlayer : int;
    location : string;
    date : DateTime;
}
with static member Initial = { nbPlayer = 0; location=""; date=DateTime.Now}

let apply state = function
    | GameCreated (gameId, username,  gameDate, gameLocation) -> { state with location = gameLocation ; date = gameDate }
    | GameJoined (gameId,username) -> { state with nbPlayer= state.nbPlayer + 1 }
    | GameAbandonned (gameId, username) -> { state with nbPlayer= state.nbPlayer - 1  }

open Validator 

module private Assert =
    let validCreateGame location date = validator (fun l -> true  ) ["the location is unkown"] location 
                                     <* validator (fun d -> true) ["the game must take place in 24 hrs at least"] date
    let validJoinGame game = validator (fun g -> g.nbPlayer <10   ) ["err:the Nb max of player is 10"] game 
    let validAbandonGame game = validator (fun g-> true) ["It is not allowed to withdraw from a game 48 hrs before the beginning"] game

    
let exec state = function
    | CreateGame (gameId,userName,  gameDate, gameLocation) -> Assert.validCreateGame gameLocation gameDate <?> GameCreated(gameId, userName,  gameDate, gameLocation) 
    | JoinGame (gameId, userName) -> Assert.validJoinGame state <?> GameJoined(gameId, userName)
    | AbandonGame (gameId, userName) -> Assert.validAbandonGame state <?> GameAbandonned(gameId,userName)
    
        
