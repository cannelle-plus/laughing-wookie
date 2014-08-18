[<RequireQualifiedAccess>]
module CommandHandler



let handleGames repo (id,v) c = (Aggregate.makeHandler { initial = Game.State.Initial; apply = Game.apply; exec = Game.exec } (repo "Game")) (id,v) c

let handlePlayers repo (id,v) c = (Aggregate.makeHandler { initial = Player.State.Initial; apply = Player.apply; exec = Player.exec } (repo "Player")) (id,v) c

