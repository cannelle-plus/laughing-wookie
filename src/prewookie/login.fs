module login

open System


let isLogged =
    let resultQuery =  database.isLoggedIn(Some("uuidGiven"))
    match resultQuery with
    | (Some(expirationDate),Some(userName)) -> if DateTime.Parse( expirationDate) > DateTime.Now then true
                                               else false
    | (_,_) -> false

