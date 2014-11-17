[<RequireQualifiedAccess>]
module CommandHandler



let handleGames repo (id,v,metadata) c = (Aggregate.makeHandler { initial = Game.State.Initial; apply = Game.apply; exec = Game.exec } (repo "Game")) (id,v,metadata) c

let handleBears repo (id,v,metadata) c = (Aggregate.makeHandler { initial = Bear.State.Initial; apply = Bear.apply; exec = Bear.exec } (repo "Bear")) (id,v,metadata) c

