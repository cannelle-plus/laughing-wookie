module Authentication

open System
open Message

type Commands =
    | IsLoggedIn of string
    | Login of string*string
    | Signin of string*string*string

let apply repoAuth = 
    let (login:string*string->(Guid*DateTime) option, saveSession, isLogged) = repoAuth
    function
    | IsLoggedIn(tokenId) -> 
        match isLogged(tokenId) with
        | Some((expirationDate,userName)) -> 
                if  expirationDate> DateTime.Now then userName 
                else "tokenExpired"
        | _ -> "unauthorized" 
    | Login(username,password) -> 
        let result = login(username,password)
        match result with 
            | Some(tokenId,expirationDate) -> 
                saveSession username tokenId expirationDate
                tokenId.ToString()
            | None ->  "invalid credentials" 
    | Signin(username,lastname,firstname) -> "to be done"


    
     
    
    

