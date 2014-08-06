module MessageHandler
open System
open Newtonsoft.Json

type Message<'TCommand> = {
    Id: Guid
    Version: int
    CorrelationId: Guid
    TokenId : Guid
    PayLoad : 'TCommand
}

let (|InvariantEqual|_|) (str:string) arg = 
  if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
    then Some() else None

let (|GameCommand|_|) eventStore commandName = 
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
    if username = "" then "not allowed"
    else handle (message.Id,message.Version) message.PayLoad

let handle repoAuth makeEventRepo message  = 
    function
    | AuthenticationCommand repoAuth handle -> 
        let msg = JsonConvert.DeserializeObject<Authentication.Commands>(message)
        handle msg
    | GameCommand makeEventRepo handle -> 
        let msg = JsonConvert.DeserializeObject<Message<Game.Commands>>(message)
        mandatoryHandlers repoAuth handle msg
    | _ -> "Command not known"

