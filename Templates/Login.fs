module OnlineConlangFront.Templates.Login

open SharedModels

open OnlineConlangFront.Foundation

open OnlineConlangFront.Templates.Loading

open Browser.Types
open Lit
open Fable.Core

[<LitElement("login-element")>]
let rec LoginElement () =
    let _ = LitElement.init (fun _ -> ())

    let username, setUsername = Hook.useState ""
    let password, setPassword = Hook.useState ""

    html $"""
        <input value={username}
            @change={fun (ev : Event) ->
                setUsername ev.target.Value
            }>
        <input value={password} type=password
            @change={fun (ev : Event) ->
                setPassword ev.target.Value
            }>
        <button
            @click={fun _ ->
                let logindata = { username = username; password = password }
                Lit.ofPromise(postLogin logindata, placeholder=loadingTemplate) |> Lit.render root
            }>
            Login
        </button>
    """

and loginTemplate =
    html $"""
        <login-element></login-element>
    """

and postLogin logindata =
    promise {
        let! stoken = server.postLogin logindata |> Async.StartAsPromise
        setJWTCookie stoken
        return loginTemplate
    }
