module OnlineConlangFront.Templates.Term

open Fable.Core
open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Import.Inflection
open OnlineConlangFront.Templates.Loading

let emptyTerm = { id = 0
                ; word = ""
                ; speechPart = ""
                ; wordClasses = Set.empty
                ; inflection = None
                ; transcription = None
                }

[<LitElement("term-table")>]
let rec TermTable () =
    let host, props =
        LitElement.init (fun init ->
            init.props <- {|
                terms = Prop.Of(Seq.empty, attribute="")
                language = Prop.Of(0)
                axes = Prop.Of(Seq.empty, attribute="")
                speechParts = Prop.Of((Seq.empty : seq<string>), attribute="")
            |}
        )

    let terms, setTerms = Hook.useState props.terms.Value
    let lid = props.language.Value
    let axes = props.axes.Value
    let speechParts = props.speechParts.Value

    let setTerm newTerm =
        let newTerms = terms |> Seq.map (fun t ->
            if t.id = newTerm.id then newTerm else t
        )
        setTerms newTerms

    let speechPartOption term sp = html $"<option ?selected={term.speechPart = sp}>{sp}</option>"
    let classP cl = html $"<p>{cl}</p>"

    let empty = [ html $"" ]
    html $"""
        <table>
            <tr>
                <th></th>
                <th>Word</th>
                <th>Part of speech</th>
                <th>Classes</th>
                <th>Inflection</th>
                <th>Transcription</th>
            </tr>
            {terms |> Seq.map (fun term ->
            html  $"<tr>
                        <td></td>
                        <td>
                            <input
                                value={term.word}
                                @keyup={fun (ev : CustomEvent) ->
                                    let newWord = ev.target.Value
                                    let newTerm = { id = term.id
                                                  ; word = newWord
                                                  ; speechPart = term.speechPart
                                                  ; wordClasses = term.wordClasses
                                                  ; inflection = term.inflection
                                                  ; transcription = term.transcription
                                                  }
                                    setTerm newTerm}>
                        </td>
                        <td>
                            <select>
                                {speechParts |> Seq.map (fun sp ->
                                    speechPartOption term sp
                                )}
                            </select>
                        </td>
                        <td>
                            {term.wordClasses |> Seq.map (fun cl ->
                                classP cl
                            )}
                        </td>
                        <td>{match term.inflection with
                                | Some termInflections -> List.map (wordInflectionTemplate axes) termInflections
                                | None -> [empty]}</td>
                        <td>{term.transcription}</td>
                        <td>
                            <button
                                @click={fun _ ->
                                    Lit.ofPromise(postRebuildInflections lid term.id, placeholder=loadingTemplate) |> Lit.render root}>
                                Refresh
                            </button>
                            <button
                                @click={fun _ ->
                                    Lit.ofPromise(putTerm lid term, placeholder=loadingTemplate) |> Lit.render root}>
                                Submit
                            </button>
                        </td>
                    </tr>")}
        </table>
    """

and putTerm lid term =
    promise {
        do! server.putTerm getJWTCookie term.id term |> Async.StartAsPromise
        return! termsTemplate lid
    }

and postRebuildInflections lid tid =
    promise {
        do! server.rebuildInflections getJWTCookie tid |> Async.StartAsPromise
        return! termsTemplate lid
    }

and termsTemplate lid =
    promise {
        let! terms = server.getTerms lid |> Async.StartAsPromise
        let! axes = server.getAxes lid |> Async.StartAsPromise
        let! speechParts = server.getSpeechParts lid |> Async.StartAsPromise
        return html   $"<term-table
                            .terms={terms}
                            .axes={axes}
                            .speechParts={speechParts}
                            language={lid}>
                        </term-table>"
    }
