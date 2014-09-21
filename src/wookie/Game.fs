
module Game

open System
open Core
open Validator 

type Commands =
    | CreateGame of string*string*DateTime*string*int
    | JoinGame 
    | AbandonGame 

type Events =
    | GameCreated of string*string*DateTime*string*int
    | GameJoined 
    | GameAbandonned 

type State = {
    nbPlayer : int;
    maxPlayer : int;
    isScheduled : bool
}
with static member Initial = { nbPlayer = 0; maxPlayer=0; isScheduled = false}

let apply state = function
    | GameCreated (name, ownerId,  gameDate, gameLocation,nbPlayersRequired) -> { state with maxPlayer =nbPlayersRequired;  isScheduled = true}
    | GameJoined  -> { state with nbPlayer= state.nbPlayer + 1 }
    | GameAbandonned  -> { state with nbPlayer= state.nbPlayer - 1  }


module private Assert =
    let validCreateGame location date game = validator (fun l -> true  ) ["the location is unkown"] location 
                                             <* validator (fun d -> true) ["the game must take place in 24 hrs at least"] date
                                             <* validator (fun g -> not g.isScheduled ) ["the game is already scheduled"] game
    let validJoinGame game = validator (fun g -> g.nbPlayer <10   ) ["err:the Nb max of player is 10"] game 
    let validAbandonGame game = validator (fun g-> true) ["It is not allowed to withdraw from a game 48 hrs before the beginning"] game


    
let exec state = function
    | CreateGame (name, ownerId,  gameDate, gameLocation, nbPlayersRequired) -> Assert.validCreateGame gameLocation gameDate state <?> GameCreated(name , ownerId,   gameDate, gameLocation,nbPlayersRequired) 
    | JoinGame  -> Assert.validJoinGame state <?> GameJoined
    | AbandonGame -> Assert.validAbandonGame state <?> GameAbandonned
    
        
