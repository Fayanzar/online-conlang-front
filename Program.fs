open Browser
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Axes
open OnlineConlangFront.Templates.Loading
open OnlineConlangFront.Templates.Term

open Fable.Core

let el = document.getElementById("root")

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
                    </tr>
                    {langs |> List.map (fun lang ->
                        html $"<tr>
                                <td>{lang.id}</td>
                                <td>{lang.name}</td>
                                <td>{termsButton lang.id}</td>
                                <td>{axesButton lang.id}</td>
                            </tr>")}
                </table>
            """
    }

JsInterop.importAll "./Templates/Loading.css"
Lit.ofPromise(indexTemplate, placeholder=loadingTemplate) |> Lit.render el
