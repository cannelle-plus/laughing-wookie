module Authentication

open System
open Message

type Commands =
    | IsLoggedIn of string
    | Login of string*string
    | Signin of string*string*string
    | LostPassword of string

let handle repoAuth = 
    let (login:string*string->(Guid*DateTime) option, saveSession, isLogged) = repoAuth
    function
    | IsLoggedIn(tokenId) -> 
        match isLogged(tokenId) with
        | Some((expirationDate,userName)) -> 
                if  expirationDate> DateTime.Now then Choice1Of2( userName )
                else Choice2Of2( "tokenExpired")
        | _ -> Choice2Of2("unauthorized" )
    | Login(username,password) -> 
        let result = login(username,password)
        match result with 
            | Some(tokenId,expirationDate) -> 
                saveSession username tokenId expirationDate
                Choice1Of2( tokenId.ToString())
            | None ->  Choice2Of2( "invalid credentials" )
    | Signin(username,lastname,firstname) -> Choice2Of2( "to be done")
    | LostPassword(username) -> Choice2Of2( "to be done")


    
     
    
    

