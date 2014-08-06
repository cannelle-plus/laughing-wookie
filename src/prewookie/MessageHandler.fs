module MessageHandler
open System
open Newtonsoft.Json
open Message


let (|InvariantEqual|_|) (str:string) arg = 
  if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
    then Some() else None

let (|GameCommand|_|) makeEventRepo commandName = 
    match commandName with 
    | InvariantEqual "JoinGame" | InvariantEqual "CreateGame" | InvariantEqual "AbandonGame"  -> 
                Some( CommandHandler.handleGames (makeEventRepo "Game") )
    | _ -> None

let (|AuthenticationCommand|_|) repoAuth commandName = 
    match commandName with 
    | InvariantEqual "Login" | InvariantEqual "Signin" ->  Some( Authentication.apply repoAuth )
    | _ -> None
    
let mandatoryHandlers repoAuth handle  message =
    let username = Authentication.apply  repoAuth (Authentication.IsLoggedIn(message.TokenId.ToString()))
    if username = "unauthorized" then Choice2Of2 ["not allowed"]
    else 
        handle (message.Id,message.Version) message.PayLoad


let handle repoAuth eventStore message  = 
    function
    | AuthenticationCommand repoAuth handle -> 
        let msg = JsonConvert.DeserializeObject<Authentication.Commands>(message)
        Choice2Of2([ handle msg])
    | GameCommand eventStore handle -> 
        let msg = JsonConvert.DeserializeObject<Command<Game.Commands>>(message)
        mandatoryHandlers repoAuth handle msg
    | _ -> Choice2Of2 ["Command not known"]

