module handler

open System 
open Newtonsoft.Json
open Message
open EventStore.ClientAPI.SystemData

let (|InvariantEqual|_|) (str:string) arg = 
  if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
    then Some() else None

type handlerType =
    | Authentication
    | Wookie


let (|PlayerAction|_|)  = function
    | InvariantEqual "Join" 
    | InvariantEqual "ScheduleGame" 
    | InvariantEqual "JoinGame" 
    | InvariantEqual "LeaveGame" 
    | InvariantEqual "CancelGame"  -> 
                Some()
    | _ -> None

let (|GameAction|_|)  = function
    | InvariantEqual "JoinGame" 
    | InvariantEqual "CreateGame" 
    | InvariantEqual "AbandonGame"  ->  Some()
    | _ -> None

let (|AuthenticationAction|_|) = function
    | InvariantEqual "Login" 
    | InvariantEqual "Signin" ->  Some()
    | _ -> None


let deserialize<'a> message = JsonConvert.DeserializeObject<Message.Command<'a>>(message)    


let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
let endPointHttp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 2113)

let conn = EventStore.conn endPoint
let user = UserCredentials("admin","changeit")

let eventStore  = EventStore.makeRepository conn Serialization.serializer user

let handleAuth (message:Message.Command<Authentication.Commands>)  = Authentication.handle sqliteConnection.repo message.PayLoad
let handlePlayer (message:Message.Command<Player.Commands>) = CommandHandler.handlePlayers eventStore (message.Id, message.Version) message.PayLoad
let handleGame (message:Message.Command<Game.Commands>) =  CommandHandler.handleGames eventStore (message.Id, message.Version) message.PayLoad


let handle message  action = 
    match action with
    | AuthenticationAction  -> 
        let result = message 
                    |> deserialize<Authentication.Commands>
                    |> handleAuth
        match result with 
        | Choice1Of2 o -> "OK"
        | Choice2Of2 o-> "KO"
    | PlayerAction ->
        let result = message
                    |> deserialize<Player.Commands>
                    |> handlePlayer
        match result with 
        | Choice1Of2 o -> "OK"
        | Choice2Of2 o-> "KO"
    | GameAction  -> 
        let result = message
                    |> deserialize<Game.Commands>
                    |> handleGame
        match result with 
        | Choice1Of2 o -> "OK"
        | Choice2Of2 o-> "KO"
    | _ -> "Command not known"
