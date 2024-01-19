module OnlineConlangFront.Templates.Term
open Browser
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Import.Inflection
open OnlineConlangFront.Templates.Loading

open Fable.Core

let el = document.getElementById("root")

let rec termTableTemplate lid axes terms =
    let empty = [ html $"" ]
    html
        $"""
            <table>
                <tr>
                    <th></th>
                    <th>Word</th>
                    <th>Part of speech</th>
                    <th>Inflection</th>
                    <th>Transcription</th>
                </tr>
                {terms |> Seq.map (fun term ->
                            html  $"<tr>
                                        <td></td>
                                        <td>{term.word}</td>
                                        <td>{term.speechPart}</td>
                                        <td>{match term.inflection with
                                                | Some inflection -> wordInflectionTemplate axes inflection
                                                | None -> empty}</td>
                                        <td>{term.transcription}</td>
                                        <td>
                                            <button
                                                @click={(fun _ ->
                                                    Lit.ofPromise(postRebuildInflections lid term.id, placeholder=loadingTemplate) |> Lit.render el)}>
                                                Refresh
                                            </button>
                                        </td>
                                    </tr>")}
            </table>
        """

and postRebuildInflections lid tid =
    promise {
        do! server.rebuildInflections tid |> Async.StartAsPromise
        return! termsTemplate lid
    }

and termsTemplate lid =
    promise {
        let! terms = server.getTerms lid |> Async.StartAsPromise
        let! axes = server.getAxes lid |> Async.StartAsPromise
        return html $"{termTableTemplate lid axes terms}"
    }
