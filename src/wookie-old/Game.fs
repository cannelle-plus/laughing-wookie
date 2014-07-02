module Game

open System
open Newtonsoft.Json
open Core

type Commands =
    | CreateGame of Guid*DateTime*string*string
    | JoinGame of Guid
    | AbandonGame of Guid

type Events =
    | GameCreated of Guid*DateTime*string*string
    | GameJoined of Guid
    | GameAbandonned of Guid
    
let execute = function
    | CreateGame(id,date,location,name) -> id.ToString() + date.tos() +l ocation +name
    | JoinGame(id) -> id
    | AbandonGame(id) -> id
    
let handleCommand jsonCmd =
    let command = JsonConvert.DeserializeObject<Commands>(jsonCmd)
    execute command
        
    
let (|GameCommand|_|) commandName = 
    match commandName with 
    | InvariantEqual "CreateGame" -> Some("CreateGame requested")
    | InvariantEqual "JoinGame" -> Some("JoinGame requested")
    | InvariantEqual "AbandonGame" -> Some("AbandonGame requested")
    | _ -> None