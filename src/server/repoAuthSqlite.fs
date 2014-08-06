[<RequireQualifiedAccess>]
module sqliteConnection

open System
open System.Configuration
open System.Linq
open FSharp.Data.Sql


[<Literal>]
let connString = @"Data Source=D:\Projects\db-wookie\db\drawTeams.db;Version=3"
[<Literal>]
let sqlitePath = @"D:\Projects\laughing-wookie\src\libs\system.data.sqlite"

// create a type alias with the connection string and database vendor settings
type sql = SqlDataProvider< 
              ConnectionString = connString,
              DatabaseVendor = Common.DatabaseProviderTypes.SQLITE,
              ResolutionPath = sqlitePath,
              IndividualsAmount = 1000,
              UseOptionTypes = true >


let ctx = sql.GetDataContext()


let repo =
    let isLoggedIn tokenId = 
        let result = query { for c in ctx.``[main].[Session]`` do
                             where ( c.tokenId = tokenId )
                             select (c.expirationDate,c.userName)
                             exactlyOneOrDefault
                            }  
        if (box result = null) then None
        else Some(DateTime.Parse(fst(result)),snd(result))
          
    let login ((username,password):string*string) = 
        let tokenid,expirationDate = query { for c in ctx.``[main].[Authentication]`` do
                                             where ( c.username = username && c.password = password )
                                             select (Guid.NewGuid(), DateTime.Now.AddMinutes(20.0))
                                             exactlyOneOrDefault
                                           }  
        

        if (box tokenid = null) then None
        else Some(tokenid,expirationDate)

    let saveSession username (tokenid:Guid) (expirationDate:DateTime) =
        let session = ctx.``[main].[Session]``.Create()
        session.tokenId <- tokenid.ToString()
        session.expirationDate <-expirationDate.ToLongTimeString()
        session.userName <- username

        ctx.SubmitUpdates()

    login,saveSession, isLoggedIn
    


    


