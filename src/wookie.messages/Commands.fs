namespace wookie.commands

open System

type CreateGame = {
    OwnerId : Guid;
    Date : DateTime;
    Location : string;
    NbPlayersRequired : int;
}



