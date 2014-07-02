// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.




#light
open System
open System.Net
open System.Text
open System.IO


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

let getData (request:HttpListenerRequest) =
    if request.HasEntityBody then
        let reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding)
        reader.ReadToEnd()
    else ""

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
            let txt = (CommandHandler.handle (getData req)) (getCommandName req)
            let txtBytes = Encoding.ASCII.GetBytes(txt)
            resp.ContentType <- "text/html"
            resp.Headers.Add("Access-Control-Allow-Origin", "*")
            resp.OutputStream.Write(txtBytes, 0, txtBytes.Length)
            resp.OutputStream.Close()
        })   

    Console.ReadKey(true) |> ignore
    0
