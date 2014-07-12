[<RequireQualifiedAccess>]
module CommandHandler

open System
open EventStore.ClientAPI.SystemData

let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
let endPointHttp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 2113)

let conn = EventStore.conn endPoint

let user = UserCredentials("admin","changeit")


let repo connection = EventStore.makeRepository connection Serialization.serializer user
let fnHandleCommand agg category connection = Aggregate.makeHandler agg (repo connection category)


let handleGames connection (id,v) c = (fnHandleCommand { initial = Game.State.Initial; apply = Game.apply; exec = Game.exec } "Game" connection) (id,v) c

