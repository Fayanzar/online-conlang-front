module OnlineConlangFront.Import.Inflection

open Lit
open Browser.Types
open Browser.CssExtensions

open SharedModels

open OnlineConlangFront.Foundation

let emptyTerm id = { id = id
                   ; word = ""
                   ; speechPart = ""
                   ; wordClasses = Set.empty
                   ; inflection = None
                   ; transcription = None
                   }

let rec private mkSpans axes =
    match axes with
    | [] -> []
    | [axis] -> [(axis, 1)]
    | axis1::axis2::xs -> (axis1, List.length axis2.values) :: mkSpans (axis2::xs)

let rec cartesian p1 p2 =
    match p1, p2 with
    | _, [] -> []
    | [], _ -> []
    | x::xs, _ -> (List.map (fun y -> x @ y) p2) @ (cartesian xs p2)

let cartesianN l =
    let lWrapped = List.map (List.map (fun l1 -> [l1])) l
    List.fold (fun acc l1 -> cartesian acc l1) [[]] lWrapped

let private mkRowOrColValues axes = axes |> List.map (fun a -> a.values |> List.map fst) |> cartesianN

[<LitElement("word-edit")>]
let WordEdit () =
    let host, props = LitElement.init (fun init ->
        init.props <- {|
            axes = Prop.Of([], attribute="")
            word = Prop.Of("", attribute="")
        |}
        init.styles <- OnlineConlangFront.Shared.styles
    )

    let axes = props.axes.Value
    let word, setWord = Hook.useState props.word.Value

    html $"""
        <input
            value={word}
            @keyup={fun (ev : CustomEvent) ->
                setWord ev.target.Value
                host.dispatchCustomEvent ("word-changed", (axes, ev.target.Value))
            }>
    """

let private mkRows rowValues colValues inflection isEditing =
    rowValues |> List.map
        (fun r ->
            colValues |> List.map
                (fun c ->
                    let (axes, word) = inflection |> List.tryFind (fun i -> Set (fst i) = Set (r @ c))
                                                  |> Option.defaultValue (Seq.empty, "")
                    if isEditing then html $"
                        <td><word-edit
                            .axes={axes |> Seq.toList}
                            .word={word}>
                        </word-edit></td>
                    "
                    else html $"<td id={axes}>{word}</td>"
                )
        )

let private mkPrefix rowValues verAxesWithSpans =
    let mutable rowValuesWithSpans = rowValues |> List.map (fun r -> List.zip r verAxesWithSpans)
    let rowLength = List.head rowValuesWithSpans |> List.length
    for i in Seq.rev {0..(rowLength - 1)} do
        let rvwsNew = rowValuesWithSpans |> List.mapi
                        (fun j r ->
                            let (_, (_, span)) = List.item i r
                            if j % span <> 0 then List.removeAt i r
                                             else r
                        )
        rowValuesWithSpans <- rvwsNew
    rowValuesWithSpans |> List.map
        (fun r ->
            r |> List.map
                (fun (v, (axes, span)) ->
                    let word = axes.values |> List.tryFind (fun (ind, _) -> ind = v) |> Option.map snd
                    html $"<th rowspan=\"{span}\">{word}</th>"
                )
        )

let rec private mkHeaderAxes horAxesWithSpans n =
    match horAxesWithSpans with
    | [] -> []
    | (axis, span)::xs -> (List.replicate n (axis, span)) :: (mkHeaderAxes xs (List.length axis.values))

[<LitElement("real-inflection-table")>]
let RealInflectionTable () =
    let _, props = LitElement.init (fun init ->
        init.styles <- OnlineConlangFront.Shared.styles
        init.props <- {|
            axes = Prop.Of([], attribute="")
            inflection = Prop.Of([], attribute="")
            isEditing = Prop.Of(false, attribute="")
        |}
    )

    let axes = props.axes.Value
    let inflection = props.inflection.Value
    let isEditing = props.isEditing.Value

    let cookieHorAxes =
        match getCookie "horizontal" with
        | None -> []
        | Some horAxes -> horAxes.Split "," |> Array.toList
    let cookieVerAxes =
        match getCookie "vertical" with
        | None -> []
        | Some verAxes -> verAxes.Split "," |> Array.toList
    let axesSorted = List.sortBy (fun axis -> List.length axis.values) axes
    let preHorAxes = cookieHorAxes
                     |> List.map (fun axisName -> List.tryFind (fun (axis : AxisForAPI) -> axis.name = axisName) axesSorted)
                     |> List.choose id
    let preVerAxes = cookieVerAxes
                     |> List.map (fun axisName -> List.tryFind (fun (axis : AxisForAPI) -> axis.name = axisName) axesSorted)
                     |> List.choose id
    let leftoverAxes = List.except (preHorAxes @ preVerAxes) axesSorted

    let (leftoverHorAxes, leftoverVerAxes) = List.foldBack (fun x (l,r) -> x::r, l) leftoverAxes ([], [])
    let horAxes = preHorAxes @ leftoverHorAxes
    let verAxes = preVerAxes @ leftoverVerAxes
    let horAxesWithSpans = mkSpans horAxes
    let verAxesWithSpans = mkSpans verAxes
    let headerAxes = mkHeaderAxes horAxesWithSpans 1
    let colNames axes = axes |> List.map (fun (axis, span) ->
        axis.values |> List.map (fun (_, name) -> html $"<th colspan=\"{span}\">{name}</th>")
    )
    let tablePrefix = mkPrefix (mkRowOrColValues verAxes) verAxesWithSpans
    let rows = mkRows (mkRowOrColValues verAxes) (mkRowOrColValues horAxes) inflection isEditing
    let tableRows = List.fold2 (fun acc l1 l2 -> acc @ [ html $" <tr>{l1 @ l2}</tr>" ]) [] tablePrefix rows

    html $"""
        <table class=inflection>
            {headerAxes |> List.map (fun axes ->
                html  $"<tr>
                            <td colspan=\"{List.length verAxes}\">
                            </td>
                            {colNames axes}
                        </tr>
                    "
                )}
            {tableRows}
        </table>
    """

[<LitElement("word-inflection-table")>]
let WordInflectionTable () =
    let host, props = LitElement.init (fun init ->
        init.styles <- [
            yield! OnlineConlangFront.Shared.styles

            css $"""
                .content {{
                    max-height: 0;
                    max-width: 0;
                    overflow: hidden;
                    transition: max-height 0.2s ease-out, max-width 0.2s ease-out;
                }}
            """
        ]
        init.props <- {|
            axes = Prop.Of([], attribute="")
            inflection = Prop.Of([], attribute="")
            inflectionName = Prop.Of(None : string Option)
            term = Prop.Of(emptyTerm 0, attribute="")
            i = Prop.Of(0, attribute="")
        |}
    )

    let axes = props.axes.Value
    let inflection, setInflection = Hook.useState props.inflection.Value
    let inflectionName = props.inflectionName.Value
    let term, setTerm = Hook.useState props.term.Value
    let i = props.i.Value

    let isEditing, setIsEditing = Hook.useState false

    let showHideInflection, setShowHideInflection = Hook.useState "Show inflection"

    let realInflectionTable = html $"""
        <real-inflection-table @word-changed={fun (ev : CustomEvent) ->
            let newInflection = ev.detail :?> (int list) * string
            let newTermInflection =
                match term.inflection with
                | None -> None
                | Some inflections -> Some (inflections |> List.mapi (fun i' (iName, (iAxes, inf)) ->
                    if i = i' then
                        let newInf = inf |> List.map (fun (k, v) ->
                            if k = fst newInflection then newInflection else (k, v)
                        )
                        setInflection newInf
                        (iName, (iAxes, newInf))
                    else (iName, (iAxes, inf))
                ))
            let newTerm =
                    { id = term.id
                    ; inflection = newTermInflection
                    ; speechPart = term.speechPart
                    ; transcription = term.transcription
                    ; word = term.word
                    ; wordClasses = term.wordClasses
                    }
            setTerm newTerm
            host.dispatchCustomEvent ("term-changed", newTerm)
        }
            .axes={axes}
            .inflection={inflection}
            .isEditing={isEditing}>
        </real-inflection-table>
    """

    let defaulWord = Option.defaultValue ([], "")

    let wordEdit axes word = html $"
        <td><word-edit
            .axes={axes |> Seq.toList}
            .word={word}>
        </word-edit></td>"

    let wordCell axes word = html $"<td id={axes}>{word}</td>"

    let realInflectionColumn axis = html $"""
        <table class=inflection @word-changed={fun (ev : CustomEvent) ->
            let newInflection = ev.detail :?> (int list) * string
            let newTermInflection =
                match term.inflection with
                | None -> None
                | Some inflections -> Some (inflections |> List.mapi (fun i' (iName, (iAxes, inf)) ->
                    if i = i' then
                        let newInf = inf |> List.map (fun (k, v) ->
                            if k = fst newInflection then newInflection else (k, v)
                        )
                        setInflection newInf
                        (iName, (iAxes, newInf))
                    else (iName, (iAxes, inf))
                ))
            let newTerm =
                    { id = term.id
                    ; inflection = newTermInflection
                    ; speechPart = term.speechPart
                    ; transcription = term.transcription
                    ; word = term.word
                    ; wordClasses = term.wordClasses
                    }
            setTerm newTerm
            host.dispatchCustomEvent ("term-changed", newTerm)
        }>
            {axis.values |> List.map (fun (v, name) ->
                html  $"<tr>
                            <th>{name}</th>
                            {   let (axes, word) = inflection |> List.tryFind (fun (key, _) -> key = [v]) |> defaulWord
                                if isEditing then wordEdit axes word else wordCell axes word
                            }
                        </tr>
                    ")}
        </table>
    """

    [ html $"""
        <button
            @click={fun (ev : Event) ->
                let t = ev.target :?> Element
                let panel = t.nextElementSibling :?> HTMLElement
                if (panel.style.maxHeight = "") then
                    Fable.Core.JS.setTimeout (fun _ ->
                        panel.style.maxHeight <- $"{panel.scrollHeight}px"
                        panel.style.maxWidth <- $"{panel.scrollWidth}px"
                        setShowHideInflection "Hide inflection"
                    ) 0
                else
                    Fable.Core.JS.setTimeout (fun _ ->
                        panel.style.maxHeight <- ""
                        panel.style.maxWidth <- ""
                        setShowHideInflection "Show inflection"
                    ) 0
            }
            >{showHideInflection}: {inflectionName}</button>
        <div class=content id=content>
            <button
                @click={fun (ev : Event) ->
                    if isEditing then setIsEditing false
                    else setIsEditing true
                    let t = ev.target :?> Element
                    let panel = t.parentElement
                    if panel.style.maxHeight <> "" then
                        Fable.Core.JS.setTimeout (fun _ ->
                            panel.style.maxWidth <- $"{panel.scrollWidth}px"
                            panel.style.maxHeight <- $"{panel.scrollHeight}px"
                        ) 0
                    else 0
                }>
                {if isEditing then "Save" else "Edit"}
            </button>
            {
                match axes with
                | [] -> html $""
                | [axis] -> realInflectionColumn axis
                | _ -> realInflectionTable
            }
        </div>
    """ ]


let wordInflectionTemplate term (axes : AxisForAPI seq) i (inflectionName, (inflectionAxes, inflection)) =
    let filteredAxes =
        axes |> Seq.filter (fun a -> List.contains a.name inflectionAxes) |> Seq.toList
    match filteredAxes with
    | [] -> [ html $"" ]
    | axesList ->
        [ html $"""
            <word-inflection-table
                inflectionName={inflectionName}
                .inflection={inflection}
                .axes={axesList}
                .term={term}
                .i={i}>
            </word-inflection-table>
        """ ]
