/// Integration with EventStore.
[<RequireQualifiedAccess>]
module EventStore

open System
open System.Net
open EventStore.ClientAPI
open EventStore.ClientAPI.SystemData

/// Creates and opens an EventStore connection.
let conn endPoint =   
    let conn = EventStoreConnection.Create(endPoint)
    conn.Connect()
    conn

    
/// Creates event store based repository.
let makeRepository (conn:IEventStoreConnection)  (serialize:obj -> string * byte array, deserialize: Type * string * byte array -> obj) (user:UserCredentials) category=

    let streamId (id:Guid) = category + "-" + id.ToString("N").ToLower()

    let load (t,id) = 
        let streamId = streamId id
        let eventsSlice = conn.ReadStreamEventsForward(streamId, 1, Int32.MaxValue, false, user) 
        eventsSlice.Events 
        |> Seq.map (fun e -> deserialize(t, e.Event.EventType, e.Event.Data))

    let commit (id,expectedVersion) e = 
        let streamId = streamId id
        let eventType,data = serialize e
        let metaData = [||] : byte array
        let eventData = new EventData(Guid.NewGuid(), eventType, true, data, metaData)
        
        conn.AppendToStream(streamId, ExpectedVersion.Any, eventData)  |> ignore
        

    load  ,commit


// Creates a function that returns a read model from the last event of a stream.
let makeReadModelGetter (conn:IEventStoreConnection) (deserialize:byte array -> _) =
    fun streamId -> async {
        let! eventsSlice = conn.ReadStreamEventsBackwardAsync(streamId, -1, 1, false) |> Async.AwaitTask
        if eventsSlice.Status <> SliceReadStatus.Success then return None
        elif eventsSlice.Events.Length = 0 then return None
        else 
            let lastEvent = eventsSlice.Events.[0]
            if lastEvent.Event.EventNumber = 0 then return None
            else return Some(deserialize(lastEvent.Event.Data))    
    }
