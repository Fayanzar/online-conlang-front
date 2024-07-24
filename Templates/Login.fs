module OnlineConlangFront.Templates.Login

open SharedModels

open OnlineConlangFront.Foundation

open OnlineConlangFront.Templates.Loading

open Browser.Types
open Browser.Url
open Browser
open Lit
open Fable.Core

[<LitElement("login-element")>]
let rec LoginElement () =
    let _ = LitElement.init (fun init ->
        init.styles <- OnlineConlangFront.Shared.styles
    )

    let username, setUsername = Hook.useState ""
    let password, setPassword = Hook.useState ""

    html $"""
        <table>
        <tr>
            <td>Username</td>
            <td>
            <input value={username}
                @change={fun (ev : Event) ->
                    setUsername ev.target.Value
                }>
            </td>
        </tr>
        <tr>
            <td>Password</td>
            <td>
            <input value={password} type=password
                @change={fun (ev : Event) ->
                    setPassword ev.target.Value
                }>
            </td>
        </tr>
        </table>
        <div>
            <button
                @click={fun _ ->
                        let logindata = { username = username; password = password }
                        Promise.map (fun _ ->
                            window.location.href <- "/"
                        ) (postLogin logindata)
                }>
                Login
            </button>
        </div>
    """

and loginTemplate =
    html $"""
        <login-element></login-element>
    """

and postLogin logindata =
    promise {
        let! stoken = server.postLogin logindata |> Async.StartAsPromise
        setJWTCookie stoken
        return ()
    }

[<LitElement("register-element")>]
let rec RegisterElement () =
    let _ = LitElement.init (fun init ->
        init.styles <- OnlineConlangFront.Shared.styles
    )

    let username, setUsername = Hook.useState ""
    let password, setPassword = Hook.useState ""
    let email, setEmail = Hook.useState ""

    html $"""
        <table>
        <tr>
            <td>Username</td>
            <td>
            <input value={username}
                @change={fun (ev : Event) ->
                    setUsername ev.target.Value
                }>
            </td>
        </tr>
        <tr>
            <td>Email</td>
            <td>
            <input value={email}
                @change={fun (ev : Event) ->
                    setEmail ev.target.Value
                }>
            </td>
        </tr>
        <tr>
            <td>Password</td>
            <td>
            <input value={password} type=password
                @change={fun (ev : Event) ->
                    setPassword ev.target.Value
                }>
            </td>
        </tr>
        </table>
        <div>
            <button
                @click={fun _ ->
                    let logindata = { username = username; password = password }
                    Lit.ofPromise(postRegister logindata email, placeholder=loadingTemplate) |> Lit.render root
                }>
                Sign Up
            </button>
        </div>
    """

and registerTemplate =
    html $"""
        <register-element></register-element>
    """

and postRegister logindata email =
    promise {
        do! server.postRegister logindata email |> Async.StartAsPromise
        return registerTemplate
    }

let verifyTemplate () =
    promise {
        let params = URLSearchParams.Create window.location.search
        let ousername = params.get "username"
        let okey = params.get "key"
        match ousername, okey with
        | Some username, Some key ->
            let! stoken = server.postVerifyUser username key |> Async.StartAsPromise
            setJWTCookie stoken
            return html $"
                <p>You are successfully verified!</p>
            "
        | _, _ -> return html $"Ooooops! Something is wrong with your request."
    }

let rec sendVerificationEmailTemplate () =
    html $"
        <button @click={fun _ ->
            Promise.map (fun _ ->
                window.location <- window.location
            ) (postSendVerificationEmail ())
        }
        >Resend verification email
        </button>
    "

and postSendVerificationEmail () =
    promise {
        do! server.postSendVerificationEmail getJWTCookie |> Async.StartAsPromise
        return ()
    }

let rec logoutTemplate () =
    html $"""
        <button @click={fun _ ->
            Promise.map (fun _ ->
                window.location <- window.location
            ) (postLogout ())
        }
        >Log out
        </button>
    """

and postLogout () =
    promise {
        let! stoken = server.postLogout getJWTCookie |> Async.StartAsPromise
        setJWTCookie stoken
        return ()
    }