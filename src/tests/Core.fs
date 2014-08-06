module Core

//let Given (events: a' list) =
open System
open Message 


let repoAuth =
    let isLoggedIn tokenId = Some(DateTime.Now,"usernameToto")
    let login ((username,password):string*string) = Some(Guid.NewGuid(),DateTime.Now)
    let saveSession username (tokenid:Guid) (expirationDate:DateTime) = "saved" |> ignore        
    login,saveSession, isLoggedIn

// string -> (Type *Guid-> seq<obj>) *(Guid*int-> obj->unit)
let makeEventRepo given (category:string)  =
    let load  (t: Type,id:Aggregate.Id ) =  given |> Seq.ofList 
    let commit (id:Aggregate.Id,v:int) msg = msg |>ignore
    load,commit



    


    


