module Core

open System
open Message 
open Newtonsoft.Json
open FsUnit.Xunit
open Xunit

let repoAuth =
    let isLoggedIn tokenId = Some(DateTime.Now.AddDays(1.0),"usernameToto")
    let login ((username,password):string*string) = Some(Guid.NewGuid(),DateTime.Now)
    let saveSession username (tokenid:Guid) (expirationDate:DateTime) = "saved" |> ignore        
    login,saveSession, isLoggedIn

let makeEventRepo evts (category:string)  =
    let load  (t: Type,id:Aggregate.Id ) : obj seq = evts
    let commit (id:Aggregate.Id,v:int) msg = msg |>ignore
    load,commit

open System
open Message 
open Newtonsoft.Json
open FsUnit.Xunit


exception AggregateException of string

let Given evts = 
    makeEventRepo (seq { for evt in evts do yield (evt:>obj) })

let execThe handle  (id,version,metadata) cmd eventStore=   
    let msg =  
        {
            Command.Id =  id
            Version = version;
            MetaData = metadata
            PayLoad = cmd
        }
    handle eventStore (msg.Id, msg.Version) msg.PayLoad


let Then (expected:Choice<'a,'b>)  (actual:Choice<'a,'b>)  = 
    match expected,actual with
    |  Choice1Of2 eventExpected, Choice1Of2 eventActual -> Xunit.Assert.Equal<'a>(eventExpected,eventActual)
    |  Choice2Of2 resultExpected ,Choice2Of2 resultActual-> Xunit.Assert.Equal<'b>(resultExpected,resultActual) 
    |  Choice1Of2 eventExpected, Choice2Of2 result -> Assert.True(false, String.Format("Expected :{0},actual :{1})",eventExpected, result))
    |  Choice2Of2 result,Choice1Of2 eventActual-> Assert.True(false, String.Format("Expected :{0},actual :{1})", result, eventActual))
    

