open Browser
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Axes
open OnlineConlangFront.Templates.Inflections
open OnlineConlangFront.Templates.Loading
open OnlineConlangFront.Templates.Rules
open OnlineConlangFront.Templates.SpeechPart
open OnlineConlangFront.Templates.Term

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
                                <td>{termsButton lang.id}</td>
                                <td>{axesButton lang.id}</td>
                                <td>{rulesButton lang.id}</td>
                                <td>{speechPartButton lang.id}</td>
                                <td>{inflectionsButton lang.id}</td>
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


JsInterop.importAll "./Templates/Loading.css"
Lit.render head homeTemplate
Lit.ofPromise(indexTemplate, placeholder=loadingTemplate) |> Lit.render root
