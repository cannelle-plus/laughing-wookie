module wookie.events

open System

type GameCreated = {
    OwnerId : Guid;
    Date : DateTime;
    Location : string;
    NbPlayersRequired : int;
}