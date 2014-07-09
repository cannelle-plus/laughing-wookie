module CommandHandler
open System
open login

let (|InvariantEqual|_|) (str:string) arg = 
  if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
    then Some() else None

let (|GameCommand|_|) commandName = 
    match commandName with 
    | InvariantEqual "CreateGame" -> Some("CreateGame requested")
    | InvariantEqual "JoinGame" -> Some("JoinGame requested")
    | InvariantEqual "AbandonGame" -> Some("AbandonGame requested")
    | _ -> None
    
let handle  command = function
    | GameCommand result-> if isLogged then "authentifie" else "not logged"
    | _ -> "Command not known"