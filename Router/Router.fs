module OnlineConlangFront.Router.Router

open Browser
open Lit

open OnlineConlangFront.Router.FormatExpressions

open System
open System.Collections.Generic
open System.Text.RegularExpressions
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives

// let url = window.location.pathname.Split "/"
//             |> Array.toList
//             |> List.filter ((=) "")

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

type Router (rootElement : string, routeUrls : (string -> Option<TemplateResult>) list) =
    let mutable root = rootElement
    let mutable routes = routeUrls
    let dispatch () =
        let rootElement = document.getElementById(root)
        let url = window.location.pathname
        let otemplate =
            routes
            |> List.choose (fun route -> route url)
            |> List.tryHead
        match otemplate with
        | None -> ()
        | Some template -> Lit.render rootElement template
    do
        window.addEventListener("load", (fun _ -> dispatch ()))
    member this.Root with get() = root and set(value) = root <- value
    member this.Routes with get() = routes and set(value) = routes <- value
