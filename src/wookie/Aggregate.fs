[<RequireQualifiedAccess>]
module Aggregate

open Core
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
        Console.WriteLine("loading Events") 
        let events = load (typeof<'TEvent>,id)
        Console.WriteLine("applying Events") 
        let state = events |> Seq.cast :> 'TEvent seq |> Seq.fold aggregate.apply aggregate.initial
        Console.WriteLine("executing Command") 
        let value = aggregate.exec state command
        match value with
        | Choice1Of2 event -> Console.WriteLine("committing events") 
                              event 
                              |> commit (id,version) |> ignore
                              Choice1Of2  event 
        | Choice2Of2 messages ->
                                Console.WriteLine("returning error message")   
                                Choice2Of2 messages 
        

