[<RequireQualifiedAccess>]
module database

open System
open System.Linq
open FSharp.Data.Sql


[<Literal>]
let connString = @"Data Source=D:\Projects\db-wookie\db\drawTeams.db;Version=3"

// create a type alias with the connection string and database vendor settings
type sql = SqlDataProvider< 
              ConnectionString = @"Data Source=D:\Projects\db-wookie\db\drawTeams.db;Version=3",
              DatabaseVendor = Common.DatabaseProviderTypes.SQLITE,
              ResolutionPath = @"C:\Program Files (x86)\System.Data.SQLite\2012\bin",
              IndividualsAmount = 1000,
              UseOptionTypes = true >
let ctx = sql.GetDataContext()

let isLoggedIn tokenId =
    query { for c in ctx.``[main].[Session]`` do
            // standard operators will work as expected; the following shows the like operator and IN operator
            where (c.tokenId =tokenId  )
            
            // arbitrarily complex projections are supported
            select (c.expirationDate,c.userName) } 
    |> Seq.exactlyOne