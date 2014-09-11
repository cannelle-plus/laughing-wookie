[<RequireQualifiedAccess>]
module sqliteConnection

open System
open System.Configuration
open System.Linq
open FSharp.Data.Sql

#if CI
[<Literal>]
let connString = @"Data Source=" + __SOURCE_DIRECTORY__ + "/../../dbRuntime/drawTeams.db;Version=3"
[<Literal>]
let sqlitePath = __SOURCE_DIRECTORY__ + @"/../libs/system.data.sqlite"
#else
[<Literal>]
let connString = @"Data Source=D:\Projects\db-wookie\db\drawTeams.db;Version=3"
[<Literal>]
let sqlitePath = @"D:\Projects\laughing-wookie\src\libs\system.data.sqlite"
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
    


    


