module OnlineConlangFront.Import.Inflection

open Lit
open Browser.Types
open Browser.CssExtensions

open SharedModels

open OnlineConlangFront.Foundation

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

let private mkRows rowValues colValues inflection =
    rowValues |> List.map
        (fun r ->
            colValues |> List.map
                (fun c ->
                    let word = inflection |> List.tryFind (fun i -> Set (fst i) = Set (r @ c)) |> Option.map snd
                    html $"<td>{word}</td>"
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

[<LitElement("word-inflection-table")>]
let InflectionTable () =
    let _, props = LitElement.init (fun init ->
        init.styles <- [
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
        |}
    )

    let axes = props.axes.Value
    let inflection = props.inflection.Value
    let inflectionName = props.inflectionName.Value

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
    let rows = mkRows (mkRowOrColValues verAxes) (mkRowOrColValues horAxes) inflection
    let tableRows = List.fold2 (fun acc l1 l2 -> acc @ [ html $" <tr>{l1 @ l2}</tr>" ]) [] tablePrefix rows
    [ html $"""
        <button
            @click={fun (ev : Event) ->
                let t = ev.target :?> Element
                let panel = t.nextElementSibling :?> HTMLElement
                if (panel.style.maxHeight = "") then
                    panel.style.maxHeight <- $"{panel.scrollHeight}px"
                    panel.style.maxWidth <- $"{panel.scrollWidth}px"
                else
                    panel.style.maxHeight <- ""
                    panel.style.maxWidth <- ""
            }
            >Show inflection: {inflectionName}</button>
        <div class=content>
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
        </div>
    """ ]

let wordInflectionTemplate (axes : AxisForAPI seq) (inflectionName, (inflectionAxes, inflection)) =
    let filteredAxes =
        axes |> Seq.filter (fun a -> List.contains a.name inflectionAxes) |> Seq.toList
    match filteredAxes with
    | [] -> [ html $"" ]
    | [axis] ->
        [ html $"""
            <table class=inflection>
                {axis.values |> List.map (fun (v, name) ->
                    html  $"<tr>
                                <th>{name}</th>
                                <td>{inflection |> List.tryFind (fun (key, _) -> key = [v]) |> Option.map snd}</td>
                            </tr>
                        ")}
            </table>
        """ ]
    | axesList ->
        [ html $"""
            <word-inflection-table
                inflectionName={inflectionName}
                .inflection={inflection}
                .axes={axesList}>
            </word-inflection-table>
        """ ]
