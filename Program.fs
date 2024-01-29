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

open OnlineConlangFront.Router.Router

open Fable.Core

let head = document.getElementById("header")

let termsButton lid =
    html
        $"""
            <button
                @click={Ev(fun _ ->
                            Lit.ofPromise(termsTemplate lid, placeholder=loadingTemplate) |> Lit.render root
                )}>
                Terms
            </button>
        """

let axesButton lid =
    html
        $"""
            <button
                @click={Ev(fun _ ->
                            Lit.ofPromise(axesTemplate lid, placeholder=loadingTemplate) |> Lit.render root
                )}>
                Axes
            </button>
        """

let rulesButton lid =
    html
        $"""
            <button
                @click={Ev(fun _ ->
                            Lit.ofPromise(rulesTemplate lid, placeholder=loadingTemplate) |> Lit.render root
                )}>
                Rules
            </button>
        """

let speechPartButton lid =
    html $"""
        <button
            @click={Ev(fun _ ->
                        Lit.ofPromise(speechPartTemplate lid, placeholder=loadingTemplate) |> Lit.render root
            )}>
            Parts of speech
        </button>
    """

let inflectionsButton lid =
    html $"""
        <button
            @click={Ev(fun _ ->
                        Lit.ofPromise(inflectionsTemplate lid, placeholder=loadingTemplate) |> Lit.render root
            )}>
            Inflections
        </button>
    """

let indexTemplate =
    promise {
        let! langs = server.getLanguages |> Async.StartAsPromise

        let termsHref lid = $"/{lid}/terms"
        let axesHref lid = $"/{lid}/axes"
        let rulesHref lid = $"/{lid}/rules"
        let speechPartHref lid = $"/{lid}/speechparts"
        let inflectionsHref lid = $"/{lid}/inflections"

        return html
            $"""
                <table>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Terms</th>
                        <th>Axes</th>
                        <th>Rules</th>
                        <th>Parts of speech</th>
                        <th>Inflections</th>
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
                            </tr>")}
                </table>
            """
    }

let homeTemplate =
    html
        $"""
            <button
                @click={Ev(fun _ ->
                    Lit.ofPromise(indexTemplate, placeholder=loadingTemplate) |> Lit.render root
                )}>
                Home
            </button>
        """

let router = new Router
                ( "root"
                , [ route "/" <| Lit.ofPromise(indexTemplate, placeholder=loadingTemplate)
                  ; routef "/%i/rules" (fun lid -> Lit.ofPromise(rulesTemplate lid, placeholder=loadingTemplate))
                  ; routef "/%i/terms" (fun lid -> Lit.ofPromise(termsTemplate lid, placeholder=loadingTemplate))
                  ; routef "/%i/speechparts" (fun lid -> Lit.ofPromise(speechPartTemplate lid, placeholder=loadingTemplate))
                  ; routef "/%i/inflections" (fun lid -> Lit.ofPromise(inflectionsTemplate lid, placeholder=loadingTemplate))
                  ; routef "/%i/axes" (fun lid -> Lit.ofPromise(axesTemplate lid, placeholder=loadingTemplate))
                  ]
                )

JsInterop.importAll "./Templates/Loading.css"
//Lit.render head homeTemplate
//Lit.ofPromise(indexTemplate, placeholder=loadingTemplate) |> Lit.render root
