module Program

open Lit
open Browser

open OnlineConlangFront.Templates.Loading

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Axes
open OnlineConlangFront.Templates.Inflections
open OnlineConlangFront.Templates.Rules
open OnlineConlangFront.Templates.SpeechPart
open OnlineConlangFront.Templates.Term
open OnlineConlangFront.Templates.Languages
open OnlineConlangFront.Templates.Login

open OnlineConlangFront.Router.Router

open Fable.Core
open OnlineConlangFront.Templates.Classes

let head = document.getElementById("header")

let indexTemplate =
    promise {
        let! langs = server.getLanguages |> Async.StartAsPromise

        let termsHref lid = $"/{lid}/terms"
        let axesHref lid = $"/{lid}/axes"
        let rulesHref lid = $"/{lid}/rules"
        let speechPartHref lid = $"/{lid}/speechparts"
        let inflectionsHref lid = $"/{lid}/inflections"
        let classesHref lid = $"/{lid}/classes"

        return html $"""
            <table>
                <tr>
                    <th>Id</th>
                    <th>Name</th>
                    <th>Terms</th>
                    <th>Axes</th>
                    <th>Rules</th>
                    <th>Parts of speech</th>
                    <th>Inflections</th>
                    <th>Classes</th>
                </tr>
                {langs |> List.map (fun lang ->
                    html $"<tr>
                            <td>{lang.id}</td>
                            <td>{lang.name}</td>
                            <td>
                                <a href={termsHref lang.id}>Terms</a>
                            </td>
                            <td>
                                <a href={axesHref lang.id}>Axes</a>
                            </td>
                            <td>
                                <a href={rulesHref lang.id}>Rules</a>
                            </td>
                            <td>
                                <a href={speechPartHref lang.id}>Parts of speech</a>
                            </td>
                            <td>
                                <a href={inflectionsHref lang.id}>Inflections</a>
                            </td>
                            <td>
                                <a href={classesHref lang.id}>Classes</a>
                            </td>
                        </tr>")}
            </table>
        """
    }

let accountDropdown () =
    let loginHref = "/login"
    let registerHref = "/signup"
    promise {
        let! user = server.getUser getJWTCookie |> Async.StartAsPromise
        match user with
        | (None, _) ->
            return html $"
                <p><a href={loginHref}>Login</a></p>
                <p><a href={registerHref}>Sign up</a></p>
            "
        | (Some (_, username), isVerified) ->
            if isVerified then
                return html $"<p>Welcome, {username}!</p>
                             {logoutTemplate ()}"
            else return html $"<p>Your account is not verified :(
                               Please check your email inbox for the verification link!</p>
                               {sendVerificationEmailTemplate ()}"
    }

let accountTemplate () =
    promise {
        let! dropdown = accountDropdown ()
        return html $"""
            <div class="dropdown">
                Account
                <div class="dropdown-content">
                    {dropdown}
                </div>
            </div>
        """
    }

let homeTemplate () =
    promise {
        let languagesHref = "/languages"
        let! account = accountTemplate ()
        return html
            $"""
                <a href="{document.location.origin}">Home</a>
                <a href={languagesHref}>Languages</a>
                {account}
            """
    }

let router = new Router
                ( "root"
                , [ route "/" <| Lit.ofPromise(indexTemplate, placeholder=loadingTemplate)
                  ; route "/login" <| loginTemplate
                  ; route "/signup" <| registerTemplate
                  ; route "/verify" <| Lit.ofPromise(verifyTemplate (), placeholder=loadingTemplate)
                  ; route "/languages" <| Lit.ofPromise(languagesTemplate (), placeholder=loadingTemplate)
                  ; routef "/%i/rules" (fun lid -> Lit.ofPromise(rulesTemplate lid, placeholder=loadingTemplate))
                  ; routef "/%i/terms" (fun lid -> Lit.ofPromise(termsTemplate lid, placeholder=loadingTemplate))
                  ; routef "/%i/speechparts" (fun lid -> Lit.ofPromise(speechPartTemplate lid, placeholder=loadingTemplate))
                  ; routef "/%i/inflections" (fun lid -> Lit.ofPromise(inflectionsTemplate lid, placeholder=loadingTemplate))
                  ; routef "/%i/axes" (fun lid -> Lit.ofPromise(axesTemplate lid, placeholder=loadingTemplate))
                  ; routef "/%i/classes" (fun lid -> Lit.ofPromise(classesTemplate lid, placeholder=loadingTemplate))
                  ]
                )

Lit.ofPromise(homeTemplate (), placeholder=loadingTemplate) |> Lit.render head
Lit.ofPromise(indexTemplate, placeholder=loadingTemplate) |> Lit.render root
