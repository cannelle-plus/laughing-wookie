// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

#light
open System
open System.Net
open System.Text
open System.IO
open System
open EventStore.ClientAPI.SystemData



let host = "http://localhost:8081/"
let siteRoot = @"C:\Mes documents\Visual Studio 2008\Projects\Drive\SiteWeb\"
let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
let endPointHttp = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 2113)

let conn = EventStore.conn endPoint
let user = UserCredentials("admin","changeit")

let makeEventRepo  = EventStore.makeRepository conn Serialization.serializer user




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

let json = "{\"Id\":\"88085239-6f0f-48c6-b73d-017333cb99bb\",\"Version\":0,\"CorrelationId\":\"88085239-6f0f-48c6-b73d-017333cb99bc\",\"TokenId\":\"88085239-6f0f-48c6-b73d-017333cb99ba\",\"PayLoad\":{\"Case\":\"CreateGame\",\"Fields\": [\"88085239-6f0f-48c6-b73d-017333cb99bb\",\"testUserName\",\"2014-12-31T10:00:00\",\"2014-12-31T09:34:12.456\",\"Toulouse\"]}}"

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
        | _ -> "unknown"

[<EntryPoint>]
let main argv = 
    listener (fun req resp ->
        async {
            let value = MessageHandler.handle sqliteConnection.repo makeEventRepo (getData req) (getCommandName req) 
            let txt = match value with 
                        | Choice1Of2 events -> "OK"
                        | Choice2Of2 messages -> messages |> List.fold (fun acc x-> acc + ";" + x) ""
            let txtBytes = Encoding.ASCII.GetBytes(txt)
            resp.ContentType <- "text/html"
            resp.Headers.Add("Access-Control-Allow-Origin", "*")
            resp.OutputStream.Write(txtBytes, 0, txtBytes.Length)
            resp.OutputStream.Close()
        })   

    Console.ReadKey(true) |> ignore
    0
