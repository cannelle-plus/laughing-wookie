[<RequireQualifiedAccess>]
module WookieServer

open System
open System.Net
open System.Text
open System.IO
open System


let host = "http://localhost:8081/"
let siteRoot = @"C:\Mes documents\Visual Studio 2008\Projects\Drive\SiteWeb\"
 

let listener (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>)) =
    let hl = new HttpListener()
    hl.Prefixes.Add host
    hl.Start()
    let task = Async.FromBeginEnd(hl.BeginGetContext, hl.EndGetContext)
    async {
        while true do
            let! context = task
            Async.Start(handler context.Request context.Response)
    } |> Async.Start

let json = "{\"Id\":\"88085239-6f0f-48c6-b73d-017333cb99bb\",\"Version\":0,\"CorrelationId\":\"88085239-6f0f-48c6-b73d-017333cb99bc\",\"TokenId\":\"88085239-6f0f-48c6-b73d-017333cb99ba\",\"PayLoad\":{\"Case\":\"CreateGame\",\"Fields\": [\"88085239-6f0f-48c6-b73d-017333cb99bb\",\"2014-12-31T10:00:00\",\"Toulouse\"]}}"

let getData (request:HttpListenerRequest) = json
//    if request.HasEntityBody then
//        let reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding)
//        reader.ReadToEnd()
//    else ""

let getCommandName (request:HttpListenerRequest) =    
    let urlFragments = request.Url.PathAndQuery.Split('/')
    match urlFragments with
        | [||] -> "welcome home"
        | [|rrrr;controller;action|] -> action + controller
        | [|rrrr;action|] -> action
        | _ -> "unknown"

let start =
    listener (fun req resp ->
        async {
            let value = handler.handle  (getData req) (getCommandName req) 

            let txtBytes = Encoding.ASCII.GetBytes(value)
            resp.ContentType <- "text/html"
            resp.Headers.Add("Access-Control-Allow-Origin", "*")
            resp.OutputStream.Write(txtBytes, 0, txtBytes.Length)
            resp.OutputStream.Close()
        })   
