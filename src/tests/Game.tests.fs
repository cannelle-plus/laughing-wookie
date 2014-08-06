module GameTests

open System
open Xunit
open FsUnit.Xunit
open Message 
open Newtonsoft.Json

exception AggregateException of string

let Given evts = Core.makeEventRepo evts
let When cmd repo  = 
    let value = MessageHandler.handle Core.repoAuth repo cmd "CreateGame"
    match value with 
    | Choice1Of2 event -> event
    | Choice2Of2 messages -> raise (AggregateException(messages |> List.fold (fun acc x-> acc + ";" + x) "" )) 
let Then = should equal 

[<Xunit.Fact>]
let createGame() = 
    let correlationId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bc")
    let gameId = System.Guid.Parse("88085239-6f0f-48c6-b73d-017333cb99bb")
    let username = "yoann"
    let date = DateTime.Parse("2014-12-31 09:34:12.456")
    let location = "Toulouse"

    let createGameExample =
        {
            Command.Id =  Guid.NewGuid();
            Version = 0;
            CorrelationId = correlationId;
            TokenId = Guid.NewGuid();
            PayLoad = Game.Commands.CreateGame( gameId,username,date,location)
        }
    let gameCreated = Game.Events.GameCreated( gameId,username,date,location)
//        {
//            Event.Id =  Guid.NewGuid();
//            Version = 0;
//            CorrelationId = correlationId;
//            PayLoad = 
//        }

    let json = JsonConvert.SerializeObject(createGameExample)

    //    let msg = JsonConvert.[<Message<Game.Commands>>(json)

    Given []
    |> When json
    |> Then gameCreated

    
    
    

   