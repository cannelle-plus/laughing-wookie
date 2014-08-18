
module Game

open System
open Core
open Validator 

type Commands =
    | CreateGame of Guid*DateTime*string 
    | JoinGame of Guid
    | AbandonGame of Guid

type Events =
    | GameCreated of Guid*DateTime*string
    | GameJoined of Guid
    | GameAbandonned of Guid
    

type State = {
    nbPlayer : int;
    location : string;
    date : DateTime;
    isScheduled : bool
}
with static member Initial = { nbPlayer = 0; location=""; date=DateTime.Now; isScheduled = false}

let apply state = function
    | GameCreated (gameId,  gameDate, gameLocation) -> { state with location = gameLocation ; date = gameDate ; isScheduled = true}
    | GameJoined (gameId) -> { state with nbPlayer= state.nbPlayer + 1 }
    | GameAbandonned (gameId) -> { state with nbPlayer= state.nbPlayer - 1  }


module private Assert =
    let validCreateGame location date game = validator (fun l -> true  ) ["the location is unkown"] location 
                                             <* validator (fun d -> true) ["the game must take place in 24 hrs at least"] date
                                             <* validator (fun g -> not g.isScheduled ) ["the game is already scheduled"] game
    let validJoinGame game = validator (fun g -> g.nbPlayer <10   ) ["err:the Nb max of player is 10"] game 
    let validAbandonGame game = validator (fun g-> true) ["It is not allowed to withdraw from a game 48 hrs before the beginning"] game


    
let exec state = function
    | CreateGame (gameId,  gameDate, gameLocation) -> Assert.validCreateGame gameLocation gameDate state <?> GameCreated(gameId,   gameDate, gameLocation) 
    | JoinGame (gameId) -> Assert.validJoinGame state <?> GameJoined(gameId)
    | AbandonGame (gameId) -> Assert.validAbandonGame state <?> GameAbandonned(gameId)
    
        
