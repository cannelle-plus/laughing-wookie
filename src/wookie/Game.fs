
module Game

open System
open Core
open Validator 

type Commands =
    | ScheduleGame of string*string*DateTime*string*int
    | JoinGame 
    | AbandonGame 

type Events =
    | GameScheduled of string*string*DateTime*string*int
    | GameJoined 
    | GameAbandonned 

type State = {
    nbBear : int;
    maxBear : int;
    isScheduled : bool
}
with static member Initial = { nbBear = 0; maxBear=0; isScheduled = false}

let apply state = function
    | GameScheduled (name, ownerId,  gameDate, gameLocation,nbBearsRequired) -> { state with maxBear =nbBearsRequired;  isScheduled = true}
    | GameJoined  -> { state with nbBear= state.nbBear + 1 }
    | GameAbandonned  -> { state with nbBear= state.nbBear - 1  }


module private Assert =
    let validScheduleGame location date game = validator (fun l -> true  ) ["the location is unkown"] location 
                                             <* validator (fun d -> true) ["the game must take place in 24 hrs at least"] date
                                             <* validator (fun g -> not g.isScheduled ) ["the game is already scheduled"] game
    let validJoinGame game = validator (fun g -> g.nbBear <10   ) ["err:the Nb max of Bear is 10"] game 
    let validAbandonGame game = validator (fun g-> true) ["It is not allowed to withdraw from a game 48 hrs before the beginning"] game


    
let exec state = function
    | ScheduleGame (name, ownerId,  gameDate, gameLocation, nbBearsRequired) -> Assert.validScheduleGame gameLocation gameDate state <?> GameScheduled(name , ownerId,   gameDate, gameLocation,nbBearsRequired) 
    | JoinGame  -> Assert.validJoinGame state <?> GameJoined
    | AbandonGame -> Assert.validAbandonGame state <?> GameAbandonned
    
        
