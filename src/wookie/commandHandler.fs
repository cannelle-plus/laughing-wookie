[<RequireQualifiedAccess>]
module CommandHandler




let handleGames repo (id,v) c = (Aggregate.makeHandler { initial = Game.State.Initial; apply = Game.apply; exec = Game.exec } repo) (id,v) c

