[<RequireQualifiedAccess>]
module WookieServer

open System
open System.Net
open System.Text
open System.IO
open System

let listener host (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>)) =
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
        | [|rrrr;aggregate|] -> aggregate
        | _ -> "unknown"

let start  host eventStoreConnection user =
    let agents = handler.Agents.Initial
    listener host (fun req resp ->
        async {
            let eventStore  = EventStore.makeRepository eventStoreConnection Serialization.serializer user
            let postedMessage = getData     req
            let aggregate = getCommandName req
 
            let value = handler.handle agents eventStore postedMessage aggregate

            let txtBytes = Encoding.ASCII.GetBytes(value)
            resp.ContentType <- "text/html"
            resp.Headers.Add("Access-Control-Allow-Origin", "*")
            resp.OutputStream.Write(txtBytes, 0, txtBytes.Length)
            resp.OutputStream.Close()
        })   
