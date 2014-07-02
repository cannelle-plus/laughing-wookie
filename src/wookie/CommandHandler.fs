module CommandHandler
open System
open Core
open Game

    
let handle  command = function
    | GameCommand result-> result
    | _ -> "Command not known"