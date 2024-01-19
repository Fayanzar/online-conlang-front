open Browser
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Axes
open OnlineConlangFront.Templates.Inflection
open OnlineConlangFront.Templates.Loading
open OnlineConlangFront.Templates.Term

open Fable.Core

let el = document.getElementById("root")
let head = document.getElementById("header")

let termsButton lid =
    html
        $"""
            <button
                @click={Ev(fun _ ->
                            Lit.ofPromise(termsTemplate lid, placeholder=loadingTemplate) |> Lit.render el
                )}>
                Terms
            </button>
        """

let axesButton lid =
    html
        $"""
            <button
                @click={Ev(fun _ ->
                            Lit.ofPromise(axesTemplate lid, placeholder=loadingTemplate) |> Lit.render el
                )}>
                Axes
            </button>
        """

let inflectionButton lid =
    html
        $"""
            <button
                @click={Ev(fun _ ->
                            Lit.ofPromise(inflectionTemplate lid, placeholder=loadingTemplate) |> Lit.render el
                )}>
                Rules
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
                    </tr>
                    {langs |> List.map (fun lang ->
                        html $"<tr>
                                <td>{lang.id}</td>
                                <td>{lang.name}</td>
                                <td>{termsButton lang.id}</td>
                                <td>{axesButton lang.id}</td>
                                <td>{inflectionButton lang.id}</td>
                            </tr>")}
                </table>
            """
    }

let homeTemplate =
    html
        $"""
            <button
                @click={Ev(fun _ ->
                    Lit.ofPromise(indexTemplate, placeholder=loadingTemplate) |> Lit.render el
                )}>
                Home
            </button>
        """


JsInterop.importAll "./Templates/Loading.css"
Lit.render head homeTemplate
Lit.ofPromise(indexTemplate, placeholder=loadingTemplate) |> Lit.render el
