module MessageHandler
open System
open login

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

let (|GameCommand|_|) connection commandName = 
    match commandName with 
    | InvariantEqual "JoinGame" | InvariantEqual "CreateGame" | InvariantEqual "AbandonGame"  -> 
                Some( CommandHandler.handleGames connection )
    | _ -> None
    
let mandatoryHandlers  handle  message =
    let username = login.isLogged(message.TokenId.ToString())
    if username = "" then "not allowed"
    else handle (message.Id,message.Version) message.PayLoad

let fromJson toto =
    {
        Message.Id = Guid.NewGuid();
        Version = 0;
        CorrelationId = Guid.NewGuid();
        TokenId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99ba");
        PayLoad = Game.Commands.CreateGame(Guid.NewGuid(),DateTime.Now,DateTime.Now,"toto")
    }
    

let handle connection message  = 
    function
    | GameCommand connection handle -> mandatoryHandlers handle (fromJson(message))
    | _ -> "Command not known"

