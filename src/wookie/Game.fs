[<RequireQualifiedAccess>]
module Game

open System
open Newtonsoft.Json
open Core
    


type Commands =
    | CreateGame of Guid*DateTime*DateTime*string
    | JoinGame of Guid
    | AbandonGame of Guid

type Events =
    | GameCreated of Guid*DateTime*DateTime*string
    | GameJoined of Guid
    | GameAbandonned of Guid

type State = {
    nbPlayer : int;
    location : string;
    date : DateTime;
}
with static member Initial = { nbPlayer = 0; location=""; date=DateTime.Now}

let apply state = function
    | GameCreated (gameId, creationDate, gameDate, gameLocation) -> { state with location = gameLocation ; date = gameDate }
    | GameJoined (gameId) -> { state with nbPlayer= state.nbPlayer + 1 }
    | GameAbandonned (gameId) -> { state with nbPlayer= state.nbPlayer - 1  }

open Validator 

module private Assert =
    let validCreateGame location date = validator (fun l -> true  ) ["the location is unkown"] location 
                                     <* validator (fun d -> true) ["the game must take place in 24 hrs at least"] date
    let validJoinGame game = validator (fun g -> g.nbPlayer <10   ) ["the Nb max of player is 10"] game 
    let validAbandonGame game = validator (fun g-> true) ["It is not allowed to withdraw from a game 48 hrs before the beginning"] game

    
let exec state = function
    | CreateGame (gameId, creationDate, gameDate, gameLocation) -> Assert.validCreateGame gameLocation gameDate <?> GameCreated(gameId, creationDate, gameDate, gameLocation) 
    | JoinGame (gameId) -> Assert.validJoinGame state <?> GameJoined(gameId)
    | AbandonGame (gameId) -> Assert.validAbandonGame state <?> GameAbandonned(gameId)
    
        
