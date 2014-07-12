//[<RequireQualifiedAccess>]
//module database
//open System
//
//let repoAuthentication =  new  readModel.repositoryAuthentication()
//
//let NullableToOption (n : System.Nullable<_>) = 
//    if n.HasValue 
//    then Some n.Value 
//    else None
//
//let isLoggedIn tokenId = 
//    let resultQuery =  repoAuthentication.getCurrentSession("uuidGiven")
//    let expirationDate = fst(resultQuery)
//    let userName = snd(resultQuery)
//    if expirationDate.HasValue && expirationDate.Value> DateTime.Now then userName
//    else ""
    

[<RequireQualifiedAccess>]
module database

open System
open System.Configuration
open System.Linq
open FSharp.Data.Sql


[<Literal>]
let connString = @"Data Source=D:\Projects\db-wookie\db\drawTeams.db;Version=3"
[<Literal>]
let sqlitePath = @"C:\Program Files (x86)\System.Data.SQLite\2013\bin"

// create a type alias with the connection string and database vendor settings
type sql = SqlDataProvider< 
              ConnectionString = connString,
              DatabaseVendor = Common.DatabaseProviderTypes.SQLITE,
              ResolutionPath = sqlitePath,
              IndividualsAmount = 1000,
              UseOptionTypes = true >

//let getContext connectionString sqlitePath = 
//    let connectionString = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).AppSettings.Settings.["ConnectionString"].Value
//
//    let sql = new SqlDataProvider< 
//                ConnectionString = connectionString,
//                DatabaseVendor = Common.DatabaseProviderTypes.SQLITE,
//                ResolutionPath = sqlitePath,
//                IndividualsAmount = 1000,
//                UseOptionTypes = true >()
//    let ctx = sql.GetDataContext()
//    ctx
                                             

let ctx = sql.GetDataContext()


let isLoggedIn tokenId = 
    let result = query { for c in ctx.``[main].[Session]`` do
                            where ( c.tokenId = tokenId )
                            // arbitrarily complex projections are supported
                            select (c.expirationDate,c.userName)
                            exactlyOneOrDefault
                          }  
    if (box result = null) then None
    else Some(result)
          


    


    

