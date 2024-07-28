module OnlineConlangFront.Foundation

open Browser

open SharedModels

open Fable.Remoting.Client

type Config = { baseUrl: string }

let settings : Config = Fable.Core.JsInterop.importMember "./settings.js"

let server : IServer =
    Remoting.createApi()
    |> Remoting.withBaseUrl settings.baseUrl
    |> Remoting.withRouteBuilder routeBuilder
    |> Remoting.buildProxy<IServer>

let root = document.getElementById("root")

let getCookie name =
    let rawCookie = document.cookie
    let kvToPair kv =
        match kv with
        | [| k; v|] -> (k, v)
        | _         -> ("", "")
    rawCookie.Split("; ")
    |> Array.map (fun s -> s.Split '=' |> kvToPair)
    |> Map.ofArray
    |> Map.tryFind name

let setCookie name value =
    document.cookie <- $"{name}={value}; SameSite=Lax; Path=/;"

let getJWTCookie = getCookie "JWT" |> Option.defaultValue "" |> SecurityToken
let setJWTCookie sjwt =
    let jwt = match sjwt with SecurityToken t -> t
    setCookie "JWT" jwt
