module Reactors

open System
open EventStore.ClientAPI
open System.Threading
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects
open Message
open EventStore.ClientAPI.SystemData


type SubscriptionDroppedException = Exception

let host = "http://localhost:8081/"

let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
let endPointHttp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 2113)

let eventStoreConnection = EventStore.conn endPoint
let user = UserCredentials("admin","changeit")


let evtAppeared  (subject:Subject<ResolvedEvent>)=
    Action<_,_>(fun (subscription:EventStoreSubscription) (event:ResolvedEvent) -> 
                subject.OnNext(event)  )

let subsciptDropped (subject:Subject<ResolvedEvent>) =
    Action<_,_,_>(fun (subscription:EventStoreSubscription) (reason:SubscriptionDropReason) (ex:Exception) -> 
                subject.OnError(ex) )

type EventStoreRxSubscription = {
    observable : Subject<ResolvedEvent> ;
    subscription : EventStoreSubscription
}

let onError (s:Subject<ResolvedEvent>) =  Action(fun() -> s.OnError(SubscriptionDroppedException()))

 
let createSubject (eventStore:IEventStoreConnection) user (resolveLinkTos:bool) =
    let subject = new Subject<ResolvedEvent>()
    eventStore.SubscribeToAllAsync(resolveLinkTos,evtAppeared subject,subsciptDropped subject, user ) |> ignore
    subject

let  whereEventTypeIs<'t> (e:ResolvedEvent) = e.OriginalEvent.EventType = typeof<'t>.ToString()
             
let subscribe<'a> observer (subject:Subject<ResolvedEvent>)  = 
    subject.Where(whereEventTypeIs<'a>).Subscribe(observer) |> ignore
    subject

let reactToGameScheduled  = 
    let cmd = Game.CreateGame("nom",Guid.NewGuid().ToString(),DateTime.Now,"Toulouse", 10)
    let metadata = { UserId="sdfs";UserName="sddsd";CorrelationId = Guid.NewGuid() }
    let message = {
        Id = Guid.NewGuid();
        Version = 0;
        MetaData = metadata
        PayLoad = cmd;
    }
    let eventStore  = EventStore.makeRepository eventStoreConnection Serialization.serializer user
    let next = Action<ResolvedEvent>(fun e -> CommandHandler.handleGames eventStore (message.Id, message.Version,message.MetaData) message.PayLoad|> ignore)
    Observer.Create(next)
   


let reactorBootstrap eventStoreConnection user =
    createSubject eventStoreConnection user false
    |> subscribe<Player.Events>(reactToGameScheduled )
        
    

               
