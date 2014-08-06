module Message
open System
    
type Command<'TCommand> = {
    Id: Guid
    Version: int
    CorrelationId: Guid
    TokenId : Guid
    PayLoad : 'TCommand
}

type Event<'TEvent> = {
    Id: Guid
    Version: int
    CorrelationId: Guid
    PayLoad : 'TEvent
}



