module OnlineConlangFront.Router.Router

open Browser
open Lit

open OnlineConlangFront.Router.FormatExpressions

let inline routef (path : PrintfFormat<_,_,_,_, 'T>)
           (routeTemplate : 'T -> TemplateResult)
           (url : string)
           : TemplateResult Option =
    tryMatchInput path MatchOptions.Exact url |> Option.map routeTemplate

let route (path : string)
          (routeTemplate : TemplateResult)
          (url : string)
          : TemplateResult Option =
    if path = url then Some routeTemplate else None

type Router ( rootElement : string
            , routeUrls : (string -> Option<TemplateResult>) list
            , t404 : TemplateResult) =
    let mutable root = rootElement
    let mutable routes = routeUrls
    let mutable template404 = t404
    let dispatch () =
        let rootElement = document.getElementById(root)
        let url = window.location.pathname
        let otemplate =
            routes
            |> List.choose (fun route -> route url)
            |> List.tryHead
        match otemplate with
        | None -> Lit.render rootElement template404
        | Some template -> Lit.render rootElement template
    do
        window.addEventListener("load", (fun _ -> dispatch ()))
    member this.Root with get() = root and set value = root <- value
    member this.Routes with get() = routes and set value = routes <- value
    member this.Template404 with get() = template404 and set value = template404 <- value
    new (rootElement, routeUrls) =
        let defTemplate404 = html $"""<h1 style="text-align: center;">404</h1>"""
        Router (rootElement, routeUrls, defTemplate404)
