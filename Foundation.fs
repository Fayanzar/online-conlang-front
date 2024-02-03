module OnlineConlangFront.Foundation

open Browser
open Fable.Core

open SharedModels

open Fable.Remoting.Client

let server : IServer =
    Remoting.createApi()
    |> Remoting.withBaseUrl "https://localhost:5001"
    |> Remoting.withRouteBuilder routeBuilder
    |> Remoting.buildProxy<IServer>

let root = document.getElementById("root")

// interface
type Window =
    // function description
    abstract alert: ?message: string -> unit

// wiring-up JavaScript and F# with [<Global>] and jsNative
let [<Global>] window: Window = jsNative

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
    document.cookie <- $"{name}={value}; SameSite=Lax"

let getJWTCookie = getCookie "JWT" |> Option.defaultValue "" |> SecurityToken
let setJWTCookie sjwt =
    let jwt = match sjwt with SecurityToken t -> t
    setCookie "JWT" jwt
