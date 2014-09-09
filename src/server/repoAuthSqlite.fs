﻿[<RequireQualifiedAccess>]
module sqliteConnection

open System
open System.Configuration
open System.Linq
open FSharp.Data.Sql

#if CI
[<Literal>]
let connString = @"Data Source=D:\TeamCity\buildAgent\work\e5447c2f206dcd6c\src\database\drawTeams.db;Version=3"
[<Literal>]
let sqlitePath = @"D:\TeamCity\buildAgent\work\e5447c2f206dcd6c\src\libs\system.data.sqlite"
#else
    #if PROD
[<Literal>]
let connString = @"Data Source=/media/yoann/data/projects/db-wookie/db/drawTeams.db;Version=3"
[<Literal>]
let sqlitePath = @"/usr/local/lib/mono/gac/Mono.Data.Sqlite/4.0.0.0__0738eb9f132ed756"
    #else
[<Literal>]
let connString = @"Data Source=D:\Projects\db-wookie\db\drawTeams.db;Version=3"
[<Literal>]
let sqlitePath = @"D:\Projects\laughing-wookie\src\libs\system.data.sqlite"
    #endif
#endif

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
        let result = query { for c in ctx.``[MAIN].[SESSION]`` do
                             where ( c.TOKENID = tokenId )
                             select (c.EXPIRATIONDATE,c.USERNAME)
                             exactlyOneOrDefault
                            }  
        if (box result = null) then None
        else Some(DateTime.Parse(fst(result)),snd(result))
          
    let login ((username,password):string*string) = 
        let tokenid,expirationDate = query { for c in ctx.``[MAIN].[AUTHENTICATION]`` do
                                             where ( c.USERNAME = username && c.PASSWORD = password )
                                             select (Guid.NewGuid(), DateTime.Now.AddMinutes(20.0))
                                             exactlyOneOrDefault
                                           }  
        

        if (box tokenid = null) then None
        else Some(tokenid,expirationDate)

    let saveSession username (tokenid:Guid) (expirationDate:DateTime) =
        let session = ctx.``[MAIN].[SESSION]``.Create()
        session.TOKENID <- tokenid.ToString()
        session.EXPIRATIONDATE <-expirationDate.ToLongTimeString()
        session.USERNAME <- username

        ctx.SubmitUpdates()

    login,saveSession, isLoggedIn
    


    


