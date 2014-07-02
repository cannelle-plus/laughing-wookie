module CommandHandler
open System
open Core
open Game

    
let handle  command = function
    | GameCommand handle-> handle command
    | _ -> "Command not known"