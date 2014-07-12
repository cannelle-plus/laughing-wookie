module login

open System


let isLogged tokenId = 
    let result = database.isLoggedIn tokenId
    match result with
    | Some((expirationDate,userName)) -> 
            if (System.DateTime.Parse( expirationDate) > DateTime.Now) then userName 
            else "tokenExpired"
    | _ -> "unauthorized" 
     
    
    

