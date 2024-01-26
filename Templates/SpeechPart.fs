module OnlineConlangFront.Templates.SpeechPart

open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Loading

open Fable.Core
open System

[<LitElement("speech-part-table")>]
let rec SpeechPartTable () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
            speechParts = Prop.Of((Seq.empty : string seq), attribute="")
        |})

    let spPairs = Seq.zip props.speechParts.Value props.speechParts.Value
    let speechParts, setSpeechParts = spPairs |> Seq.toList |> Hook.useState
    let lid = props.language.Value
    let basicPlaceholder = "$placeholder$"
    let placeholder uuid = $"$placeholder$-{uuid}"

    let setSpeechPart oldSP newSP =
        let newSpeechParts = speechParts |> List.map (fun sp ->
            if fst sp = fst oldSP then (fst sp, newSP) else sp
        )
        setSpeechParts newSpeechParts

    html $"""
        <table>
            <tr>
                <th>Parts of speech</th>
            </tr>
            {speechParts |> Seq.map (fun sp ->
            html  $"<tr>
                        <td>
                            <input
                                value={snd sp}
                                @keyup={fun (ev : CustomEvent) ->
                                    setSpeechPart sp ev.target.Value
                                }>
                        </td>
                        <td>
                            <button
                                @click={fun _ ->
                                    if (fst sp).Contains basicPlaceholder
                                        then Lit.ofPromise(postSpeechPart lid (snd sp), placeholder=loadingTemplate) |> Lit.render root
                                        else Lit.ofPromise(putSpeechPart lid (fst sp) (snd sp), placeholder=loadingTemplate) |> Lit.render root
                                    }>
                                Submit
                            </button>
                        </td>
                    </tr>"
            )}
        </table>
        <button
            @click={fun _ ->
                let uuid = Guid.NewGuid()
                setSpeechParts (speechParts @ [(placeholder uuid, "")])
            }>
            Add part of speech
        </button>
    """

and postSpeechPart lid sp =
    promise {
        do! server.postSpeechPart lid sp |> Async.StartAsPromise
        return! speechPartTemplate lid
    }

and putSpeechPart lid oldSP newSP =
    promise {
        window.alert($"{lid} {oldSP} {newSP}")
        do! server.putSpeechPart lid oldSP newSP |> Async.StartAsPromise
        return! speechPartTemplate lid
    }

and speechPartTemplate lid =
    promise {
        let! speechParts = server.getSpeechParts lid |> Async.StartAsPromise
        return html $"<speech-part-table
                language={lid}
                .speechParts={speechParts}
            ></speech-part-table>"
    }
