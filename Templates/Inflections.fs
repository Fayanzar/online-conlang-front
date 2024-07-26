module OnlineConlangFront.Templates.Inflections

open Browser.Types
open Lit
open Fable.Core

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Loading

open Browser.Dom

let emptyInflection = { inflectionSpeechPart = ""; inflectionAxes = []; inflectionClasses = []; inflectionName = None }

[<LitElement("inflection-axes")>]
let InflectionAxes () =
    let host, props = LitElement.init (fun init ->
        init.props <- {|
            inflectionAxes = Prop.Of(([] : list<int>), attribute="")
            axes = Prop.Of(([] : list<int * string>), attribute="")
        |}
        init.styles <- OnlineConlangFront.Shared.styles
    )
    let inflectionAxes, setInflectionAxes = Hook.useState props.inflectionAxes.Value
    let axes = props.axes.Value
    let defaultAxis = 0

    let axesOption iAxis axis = html $"<option ?selected={fst axis = iAxis}>{snd axis}</option>"

    let onAxesChanged newAxes = host.dispatchCustomEvent("axes-changed", newAxes)

    let findAxisId name =
        axes |> List.tryFind (fun (_, axis) -> axis = name) |> Option.defaultValue (0, "")

    html $"""
        {inflectionAxes |> List.mapi (fun i iAxis ->
            html  $"<div>
                        <div class=\"select\">
                        <select
                            @change={fun (ev : CustomEvent) ->
                                let newAxis = ev.target.Value |> findAxisId |> fst
                                let newAxes = inflectionAxes |> List.mapi (fun i' axis' ->
                                    if iAxis = axis' && i = i' then newAxis else axis'
                                )
                                setInflectionAxes newAxes
                                onAxesChanged newAxes
                            }>
                            <option>--Select axis--</option>
                            {axes |> List.map (fun axis -> axesOption iAxis axis)}
                        </select>
                        </div>
                        <button
                            @click={fun _ ->
                                let newAxes = inflectionAxes |> List.removeAt i
                                setInflectionAxes newAxes
                                onAxesChanged newAxes
                            }>
                            ❌
                        </button>
                    </div>
            "
        )}
        <div>
            <button
                @click={fun _ ->
                    let newAxes = inflectionAxes @ [defaultAxis]
                    setInflectionAxes newAxes
                    onAxesChanged newAxes
                }>
                Add axis
            </button>
        </div>
    """

[<LitElement("inflection-classes")>]
let InflectionClasses () =
    let host, props = LitElement.init (fun init ->
        init.props <- {|
            inflectionClasses = Prop.Of(([] : list<string>), attribute="")
            classes = Prop.Of(([] : list<string>), attribute="")
        |}
        init.styles <- OnlineConlangFront.Shared.styles
    )
    let inflectionClasses, setInflectionClasses = Hook.useState props.inflectionClasses.Value
    let classes = props.classes.Value
    let defaultClass = ""

    let classOption iClass cl = html $"<option ?selected={cl = iClass}>{cl}</option>"

    let onClassesChanged newClasses = host.dispatchCustomEvent("classes-changed", newClasses)

    html $"""
        {inflectionClasses |> List.mapi (fun i iClass ->
            html  $"<div>
                        <div class=\"select\">
                        <select
                            @change={fun (ev : CustomEvent) ->
                                let newClasses = inflectionClasses |> List.mapi (fun i' c' ->
                                    if c' = iClass && i = i' then ev.target.Value else c'
                                )
                                setInflectionClasses newClasses
                                onClassesChanged newClasses
                            }>
                            <option>--Select class--</option>
                            {classes |> List.map (fun cl -> classOption iClass cl)}
                        </select>
                        </div>
                        <button
                            @click={fun _ ->
                                let newClasses = inflectionClasses |> List.removeAt i
                                setInflectionClasses newClasses
                                onClassesChanged newClasses
                            }>
                            ❌
                        </button>
                    </div>
            "
        )}
        <div>
            <button
                @click={fun _ ->
                    let newClasses = inflectionClasses @ [defaultClass]
                    setInflectionClasses newClasses
                    onClassesChanged newClasses
                }>
                Add class
            </button>
        </div>
    """

[<LitElement("inflection-table")>]
let rec InflectionElement () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
            inflections = Prop.Of([], attribute="")
            axes = Prop.Of((Seq.empty : seq<AxisForAPI>), attribute="")
            classes = Prop.Of(([] : list<string>), attribute="")
            speechParts = Prop.Of(([] : seq<string>), attribute="")
        |}
        init.styles <- OnlineConlangFront.Shared.styles
    )

    let oldInflectionIndices = props.inflections.Value |> List.map fst

    let lid = props.language.Value
    let axes = props.axes.Value
    let classes = props.classes.Value
    let inflections, setInflections = Hook.useState props.inflections.Value
    let speechParts = props.speechParts.Value

    let setInflection k newInflection =
        let newInflections = inflections |> List.map (fun (k', i) ->
            if k' = k then (k, newInflection) else (k', i)
        )
        setInflections newInflections

    let removeInflection k =
        let newInflections = inflections |> List.filter ((<>) k << fst)
        setInflections newInflections

    let spOption iSp sp = html $"<option ?selected={iSp = sp}>{sp}</option>"

    html $"""
        <table>
            <tr>
                <th>Id</th>
                <th>Name</th>
                <th>Part of speech</th>
                <th>Classes</th>
                <th>Axes</th>
            </tr>
            {inflections |> List.map (fun ((k : int), inflection) ->
                html  $"<tr>
                            <td>{k}</td>
                            <td>
                                <input
                                    value={inflection.inflectionName}
                                    @keyup={fun (ev : CustomEvent) ->
                                        let newInflection =
                                            { inflectionSpeechPart = inflection.inflectionSpeechPart
                                            ; inflectionName = Some ev.target.Value
                                            ; inflectionAxes = inflection.inflectionAxes
                                            ; inflectionClasses = inflection.inflectionClasses
                                            }
                                        setInflection k newInflection
                                    }>
                            </td>
                            <td>
                                <div class=\"select\">
                                <select
                                    @change={fun (ev : CustomEvent) ->
                                        let newInflection =
                                            { inflectionSpeechPart = ev.target.Value
                                            ; inflectionName = inflection.inflectionName
                                            ; inflectionAxes = inflection.inflectionAxes
                                            ; inflectionClasses = inflection.inflectionClasses
                                            }
                                        setInflection k newInflection
                                    }>
                                    <option>--Select part of speech--</option>
                                    {speechParts |> Seq.map (fun sp ->
                                        spOption inflection.inflectionSpeechPart sp
                                    )}
                                </select>
                                </div>
                            </td>
                            <td>
                                <inflection-classes
                                    @classes-changed={fun (ev : CustomEvent) ->
                                        let newClasses = ev.detail :?> string list
                                        let newInflection =
                                            { inflectionSpeechPart = inflection.inflectionSpeechPart
                                            ; inflectionName = inflection.inflectionName
                                            ; inflectionAxes = inflection.inflectionAxes
                                            ; inflectionClasses = newClasses
                                            }
                                        setInflection k newInflection
                                    }
                                    .inflectionClasses={inflection.inflectionClasses}
                                    .classes={classes}>
                                </inflection-classes>
                            </td>
                            <td>
                                <inflection-axes
                                    @axes-changed={fun (ev : CustomEvent) ->
                                        let newAxes = ev.detail :?> int list
                                        let newInflection =
                                            { inflectionSpeechPart = inflection.inflectionSpeechPart
                                            ; inflectionName = inflection.inflectionName
                                            ; inflectionAxes = newAxes
                                            ; inflectionClasses = inflection.inflectionClasses
                                            }
                                        setInflection k newInflection

                                    }
                                    .inflectionAxes={inflection.inflectionAxes}
                                    .axes={axes |> Seq.map (fun a -> (a.id, a.name)) |> Seq.toList}>
                                </inflection-axes>
                            </td>
                            <td>
                                <button
                                    @click={fun _ ->
                                        if not (List.contains k oldInflectionIndices) then
                                            removeInflection k
                                        else
                                            Lit.ofPromise(deleteInflection lid k, placeholder=loadingTemplate) |> Lit.render root
                                    }>
                                    ❌
                                </button>
                                <button
                                    @click={fun _ ->
                                        console.log inflection
                                        if not (List.contains k oldInflectionIndices) then
                                            Lit.ofPromise(postInflection lid inflection, placeholder=loadingTemplate) |> Lit.render root
                                        else
                                            Lit.ofPromise(putInflection lid k inflection, placeholder=loadingTemplate) |> Lit.render root
                                    }>
                                    Submit
                                </button>
                            </td>
                        </tr>
                "
            )}
        </table>
        <button
            @click={fun _ ->
                let maxInd =
                    match inflections |> List.map fst with
                    | [] -> 0
                    | indices  -> List.max indices
                setInflections (inflections @ [(maxInd + 1, emptyInflection)])
            }>
            Add inflection
        </button>
    """

and putInflection lid iid inflection =
    promise {
        do! server.putInflection getJWTCookie iid inflection |> Async.StartAsPromise
        return! inflectionsTemplate lid
    }

and postInflection lid inflection =
    promise {
        do! server.postInflection getJWTCookie inflection |> Async.StartAsPromise
        return! inflectionsTemplate lid
    }

and deleteInflection lid iid =
    promise {
        do! server.deleteInflection getJWTCookie iid |> Async.StartAsPromise
        return! inflectionsTemplate lid
    }

and inflectionsTemplate lid =
    promise {
        let! inflections = server.getInflectionsStructure lid |> Async.StartAsPromise
        let! speechParts = server.getSpeechParts lid |> Async.StartAsPromise
        let! classes = server.getClasses lid |> Async.StartAsPromise
        let! axes = server.getAxes lid |> Async.StartAsPromise
        return html $"
            <inflection-table
                language={lid}
                .inflections={inflections |> Map.toList}
                .axes={axes}
                .classes={classes |> Map.values |> Seq.concat |> Seq.toList}
                .speechParts={speechParts}>
            </inflection-table>"
    }
