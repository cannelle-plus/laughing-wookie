module login

open System


let isLogged tokenId = 
    let result = database.isLoggedIn tokenId
    match result with
    | Some((expirationDate,userName)) -> userName
    | _ -> "" 
     
    
    

