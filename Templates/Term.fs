module OnlineConlangFront.Templates.Term

open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Import.Inflection

open Fable.Core

let termTableTemplate axes terms =
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
                                    </tr>")}
            </table>
        """

let termsTemplate lid =
    promise {
        let! terms = server.getTerms lid |> Async.StartAsPromise
        let! axes = server.getAxes lid |> Async.StartAsPromise
        return html $"<p>{termTableTemplate axes terms}</p>"
    }
