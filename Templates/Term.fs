module OnlineConlangFront.Templates.Term

open Fable.Core
open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Import.Inflection
open OnlineConlangFront.Templates.Loading

let emptyTerm id = { id = id
                   ; word = ""
                   ; speechPart = ""
                   ; wordClasses = Set.empty
                   ; inflection = None
                   ; transcription = None
                   }

[<LitElement("class-selector")>]
let ClassSelector () =
    let host, props =
        LitElement.init (fun init ->
            init.props <- {|
                classes = Prop.Of((Seq.empty : seq<string>), attribute="")
                termClasses = Prop.Of(([]: Class list), attribute="")
            |}
            init.styles <- OnlineConlangFront.Shared.styles
        )

    let termClasses, setTermClasses = Hook.useState props.termClasses.Value
    let classes = props.classes.Value
    let defaultClass = ""

    let classOption termCl cl = html $"<option ?selected={termCl = cl}>{cl}</option>"

    let onClassesChanged newClasses = host.dispatchCustomEvent("classes-changed", newClasses)

    html $"""
        {termClasses |> List.mapi (fun i termCl ->
            html $"
                <div class=\"select\">
                    <select
                        @change={fun (ev : CustomEvent) ->
                            let newClasses = termClasses |> List.mapi (fun i' cl' ->
                                if cl' = termCl && i = i' then ev.target.Value else cl'
                            )
                            setTermClasses newClasses
                            onClassesChanged newClasses
                        }>
                        <option>--Select class--</option>
                        {classes |> Seq.map (classOption termCl)}
                    </select>
                </div>
                <button
                    @click={fun _ ->
                        let newClasses = termClasses |> List.removeAt i
                        setTermClasses newClasses
                        onClassesChanged newClasses
                    }>
                    ❌
                </button>
            "
        )}
        <div>
            <button
                @click={fun _ ->
                    let newClasses = termClasses @ [defaultClass]
                    setTermClasses newClasses
                    onClassesChanged newClasses
                }>
                Add class
            </button>
        </div>
    """

[<LitElement("term-table")>]
let rec TermTable () =
    let _, props =
        LitElement.init (fun init ->
            init.props <- {|
                terms = Prop.Of(Seq.empty, attribute="")
                language = Prop.Of(0)
                axes = Prop.Of(Seq.empty, attribute="")
                speechParts = Prop.Of((Seq.empty : seq<string>), attribute="")
                classes = Prop.Of((Seq.empty : seq<string>), attribute="")
            |}
            init.styles <- OnlineConlangFront.Shared.styles
        )

    let terms, setTerms = props.terms.Value |> Seq.map (fun t -> (false, t)) |> Hook.useState
    let lid = props.language.Value
    let axes = props.axes.Value
    let classes = props.classes.Value
    let speechParts = props.speechParts.Value

    let setTerm newTerm =
        let newTerms = terms |> Seq.map (fun (b, t) ->
            if t.id = newTerm.id then (b, newTerm) else (b, t)
        )
        setTerms newTerms

    let removeTerm term =
        let newTerms = Seq.except [(true, term)] terms
        setTerms newTerms

    let speechPartOption term sp = html $"<option ?selected={term.speechPart = sp}>{sp}</option>"

    let refreshButton term = html $"
        <button
            @click={fun _ ->
                Lit.ofPromise(postRebuildInflections lid term.id, placeholder=loadingTemplate) |> Lit.render root}>
            Refresh
        </button>
    "

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
            {terms |> Seq.map (fun (b, term) ->
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
                        <div class=\"select\">
                            <select
                                @change={fun (ev : CustomEvent) ->
                                    let newTerm =
                                            { id = term.id
                                            ; word = term.word
                                            ; speechPart = ev.target.Value
                                            ; wordClasses = term.wordClasses
                                            ; inflection = term.inflection
                                            ; transcription = term.transcription
                                            }
                                    setTerm newTerm
                                }>
                                {speechParts |> Seq.map (fun sp ->
                                    speechPartOption term sp
                                )}
                            </select>
                        </div>
                        </td>
                        <td>
                            <class-selector
                                @classes-changed={fun (ev : CustomEvent) ->
                                    let newClasses = ev.detail :?> string list
                                    let newTerm =
                                            { id = term.id
                                            ; word = term.word
                                            ; speechPart = term.speechPart
                                            ; wordClasses = newClasses |> Set.ofList
                                            ; inflection = term.inflection
                                            ; transcription = term.transcription
                                            }
                                    setTerm newTerm
                                }
                                .classes={classes}
                                .termClasses={term.wordClasses |> Set.toList}>
                            </class-selector>
                        </td>
                        <td>{match term.inflection with
                                | Some termInflections -> List.map (wordInflectionTemplate axes) termInflections
                                | None -> [empty]}</td>
                        <td>{term.transcription}</td>
                        <td>
                            {if not b then [refreshButton term] else []}
                            <button
                                @click={fun _ ->
                                    if b then Lit.ofPromise(postTerm lid term, placeholder=loadingTemplate) |> Lit.render root
                                    else Lit.ofPromise(putTerm lid term, placeholder=loadingTemplate) |> Lit.render root}>
                                Submit
                            </button>
                            <button
                                @click={fun _ ->
                                    if b then removeTerm term
                                    else Lit.ofPromise(deleteTerm lid term.id, placeholder=loadingTemplate) |> Lit.render root
                            }>
                            ❌
                            </button>
                        </td>
                    </tr>")}
        </table>
        <button @click={fun _ ->
            let maxId = terms |> Seq.toList |> List.map (fun (_, t) -> t.id) |> function
                | []   -> 0
                | tSeq -> List.max tSeq
            (Seq.toList terms) @ [(true, emptyTerm <| maxId + 1)] |> List.toSeq |> setTerms
        }
        >Add term
        </button>
    """

and putTerm lid term =
    promise {
        do! server.putTerm getJWTCookie term.id term |> Async.StartAsPromise
        return! termsTemplate lid
    }

and postTerm lid term =
    promise {
        do! server.postTerm getJWTCookie lid term |> Async.StartAsPromise
        return! termsTemplate lid
    }

and deleteTerm lid tid =
    promise {
        do! server.deleteTerm getJWTCookie tid |> Async.StartAsPromise
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
        let! classes = server.getClasses lid |> Async.StartAsPromise
        let! speechParts = server.getSpeechParts lid |> Async.StartAsPromise
        return html   $"<term-table
                            .terms={terms}
                            .axes={axes}
                            .speechParts={speechParts}
                            .classes={classes |> Map.values |> Seq.concat}
                            language={lid}>
                        </term-table>"
    }
