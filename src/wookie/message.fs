module Message
open System
    
type Metadata = {
    UserId : string;
    UserName : string;
    CorrelationId : Guid;
}

type Command<'TCommand> = {
    Id: Guid
    Version: int
    MetaData : Metadata
    PayLoad : 'TCommand
}

type Event<'TEvent> = {
    Id: Guid
    Version: int
    MetaData : Metadata
    PayLoad : 'TEvent
}



