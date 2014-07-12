module MessageHandler
open System
open login
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

let (|GameCommand|_|) connection commandName = 
    match commandName with 
    | InvariantEqual "JoinGame" | InvariantEqual "CreateGame" | InvariantEqual "AbandonGame"  -> 
                Some( CommandHandler.handleGames connection )
    | _ -> None
    
let mandatoryHandlers  handle  message =
    let username = login.isLogged(message.TokenId.ToString())
    if username = "" then "not allowed"
    else handle (message.Id,message.Version) message.PayLoad

let handle connection message  = 
//    let createGameExample =
//        {
//            Message.Id =  System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb");
//            Version = 0;
//            CorrelationId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bc");
//            TokenId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99ba");
//            PayLoad = Game.Commands.CreateGame(System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb"),DateTime.Parse("2014-12-31 10:00:00.000"),DateTime.Parse("2014-12-31 09:34:12.456"),"Toulouse")
//        }
//
//    let json = JsonConvert.SerializeObject(createGameExample)
//

//    let msg = JsonConvert.DeserializeObject<Message<Game.Commands>>(Message)

//    System.Diagnostics.Debug.WriteLine(json)

    function
    | GameCommand connection handle -> 
        let msg = JsonConvert.DeserializeObject<Message<Game.Commands>>(message)
        mandatoryHandlers handle msg
    | _ -> "Command not known"

