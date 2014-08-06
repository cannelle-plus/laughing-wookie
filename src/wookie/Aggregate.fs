[<RequireQualifiedAccess>]
module Aggregate

open System 

type Aggregate<'TState, 'TCommand,'TEvent> = {
    initial : 'TState;
    apply : 'TState->'TEvent->'TState;
    exec : 'TState -> 'TCommand->Choice<'TEvent, string list>;
}


type Id = System.Guid

/// Creates a persistent command handler for an aggregate.
let makeHandler (aggregate:Aggregate<'TState, 'TCommand, 'TEvent>) (load:System.Type * Id -> obj seq, commit:Id * int -> obj -> unit) =
    fun (id,version) command -> 
        let events = load (typeof<'TEvent>,id)
        let state = events |> Seq.cast :> 'TEvent seq |> Seq.fold aggregate.apply aggregate.initial
        let value = aggregate.exec state command
        match value with
        | Choice1Of2 event -> event 
                              |> commit (id,version) 
                              Choice1Of2 event 
        | Choice2Of2 messages ->  Choice2Of2 messages  
        
    

