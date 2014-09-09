module handler

open System 
open Newtonsoft.Json
open Message

let (|InvariantEqual|_|) (str:string) arg = 
  if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
    then Some() else None

type handlerType =
    | Authentication
    | Wookie

type postingMessage<'TMessage> =
    | WithReply of 'TMessage * AsyncReplyChannel<Choice<string,string>>
    | NoReply of 'TMessage


type postingCommand<'TCommandMessage,'TEvent> =
    | WithReply of 'TCommandMessage * AsyncReplyChannel<Choice<'TEvent,string list>>
    | NoReply of 'TCommandMessage

let (|PlayerAction|_|)  = function
    | InvariantEqual "Player" ->
//    | InvariantEqual "Join" 
//    | InvariantEqual "ScheduleGame" 
//    | InvariantEqual "JoinGame" 
//    | InvariantEqual "LeaveGame" 
//    | InvariantEqual "CancelGame"  -> 
                Some()
    | _ -> None

let (|GameAction|_|)  = function
    | InvariantEqual "Game" 
//    | InvariantEqual "JoinGame" 
//    | InvariantEqual "CreateGame" 
//    | InvariantEqual "AbandonGame" 
         ->  Some()
    | _ -> None

let (|AuthenticationAction|_|) = function
    | InvariantEqual "Authentication" ->  Some()
//    | InvariantEqual "Login" 
//    | InvariantEqual "Logout" 
//    | InvariantEqual "Signin" 
    | _ -> None


let deserialize<'a> message = JsonConvert.DeserializeObject<Message.Command<'a>>(message)    

let handlePlayer eventStore (message:Message.Command<Player.Commands>) = CommandHandler.handlePlayers eventStore (message.Id, message.Version) message.PayLoad
let handleGame eventStore (message:Message.Command<Game.Commands>) =  CommandHandler.handleGames eventStore (message.Id, message.Version) message.PayLoad

let agentAuth = MailboxProcessor<postingMessage<Message.Command<Authentication.Commands>>>.Start(fun inbox ->
    let rec loop  =
        async {
                let! messageReceived = inbox.Receive();
                match messageReceived with 
                | postingMessage.WithReply(msg,replyChannel) -> 
                    let result = Authentication.handle sqliteConnection.repo msg.PayLoad
                    replyChannel.Reply(result)
                | postingMessage.NoReply(msg) -> Authentication.handle sqliteConnection.repo msg.PayLoad |> ignore
                
                do! loop
        }
    loop )

let agentPlayer eventStore = MailboxProcessor<postingCommand<Message.Command<Player.Commands>,Player.Events>>.Start(fun inbox ->
    let rec loop  =
        async {
                let! messageReceived = inbox.Receive();
                match messageReceived with 
                | postingCommand.WithReply(msg,replyChannel) -> 
                    Console.WriteLine("Agent handling players")
                    let result = CommandHandler.handlePlayers eventStore (msg.Id, msg.Version) msg.PayLoad
                    replyChannel.Reply(result)
                | postingCommand.NoReply(msg) -> CommandHandler.handlePlayers eventStore (msg.Id, msg.Version) msg.PayLoad |> ignore
                
                do! loop
        }
    loop )

let agentGame eventStore = MailboxProcessor<postingCommand<Message.Command<Game.Commands>,Game.Events>>.Start(fun inbox ->
    let rec loop  =
        async {
                let! messageReceived = inbox.Receive();
                match messageReceived with 
                | postingCommand.WithReply(msg,replyChannel) ->
                    Console.WriteLine("Agent handling games") 
                    let result = CommandHandler.handleGames eventStore (msg.Id, msg.Version) msg.PayLoad
                    replyChannel.Reply(result)
                | postingCommand.NoReply(msg) -> CommandHandler.handleGames eventStore (msg.Id, msg.Version) msg.PayLoad |> ignore
                
                do! loop
        }
    loop )

type Agents = {
    Players : System.Collections.Generic.Dictionary<Guid,MailboxProcessor<postingCommand<Message.Command<Player.Commands>,Player.Events>>>;
    Games : System.Collections.Generic.Dictionary<Guid,MailboxProcessor<postingCommand<Message.Command<Game.Commands>,Game.Events>>>;
}
with static member Initial = { 
                                 Players = new System.Collections.Generic.Dictionary<Guid,MailboxProcessor<postingCommand<Message.Command<Player.Commands>,Player.Events>>>();
                                 Games = new System.Collections.Generic.Dictionary<Guid,MailboxProcessor<postingCommand<Message.Command<Game.Commands>,Game.Events>>>();
                             }

let handle agents eventStore  message  action = 
    
    match action with
    | AuthenticationAction  -> 
        Console.WriteLine("Authentication")
        let msg = message |> deserialize<Authentication.Commands>
        let result =agentAuth.PostAndReply(fun replyChannel -> postingMessage.WithReply( msg,replyChannel))
        match result with 
        | Choice1Of2 o -> "OK"
        | Choice2Of2 o-> "KO"
    | PlayerAction ->
        Console.WriteLine("Player")
        let msg = message |> deserialize<Player.Commands>
        if not(agents.Players.ContainsKey(msg.Id)) then
            Console.WriteLine("creating Agent")
            agents.Players.[msg.Id] <- agentPlayer eventStore
            Console.WriteLine("Agent created")
        let result = agents.Players.[msg.Id].PostAndReply(fun replyChannel -> postingCommand.WithReply( msg,replyChannel))
        match result with 
        | Choice1Of2 o -> "OK"
        | Choice2Of2 o-> "KO"
    | GameAction  -> 
        Console.WriteLine("Game")
        let msg = message |> deserialize<Game.Commands>
        if not(agents.Games.ContainsKey(msg.Id)) then
            Console.WriteLine("creating Agent")
            agents.Games.[msg.Id] <- agentGame eventStore
            Console.WriteLine("Agent created")
        let result = agents.Games.[msg.Id].PostAndReply(fun replyChannel -> postingCommand.WithReply( msg,replyChannel))
        match result with 
        | Choice1Of2 o -> "OK"
        | Choice2Of2 o-> "KO"
    | _ -> "Command not known"
