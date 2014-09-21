[<RequireQualifiedAccess>]
module CommandHandler



let handleGames repo (id,v,metadata) c = (Aggregate.makeHandler { initial = Game.State.Initial; apply = Game.apply; exec = Game.exec } (repo "Game")) (id,v,metadata) c

let handlePlayers repo (id,v,metadata) c = (Aggregate.makeHandler { initial = Player.State.Initial; apply = Player.apply; exec = Player.exec } (repo "Player")) (id,v,metadata) c

