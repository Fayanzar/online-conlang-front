module OnlineConlangFront.Import.Inflection

open Lit

open SharedModels

let rec private mkSpans axes =
    match axes with
    | [] -> []
    | [axis] -> [(axis, 1)]
    | axis1::axis2::xs -> (axis1, List.length axis2.values) :: mkSpans xs

let rec private cartesian p1 p2 =
    match p1, p2 with
    | _, [] -> []
    | [], _ -> []
    | x::xs, _ -> (List.map (fun y -> x @ y) p2) @ (cartesian xs p2)

let private cartesianN l =
    let lWrapped = List.map (List.map (fun l1 -> [l1])) l
    List.fold (fun acc l1 -> cartesian acc l1) [[]] lWrapped

let private mkRowOrColValues axes = axes |> List.map (fun a -> a.values |> List.map fst) |> cartesianN

let rec private mergeRowColumn row column =
    match row, column with
    | [], [] -> []
    | _::_, [] -> row
    | [], _::_ -> column
    | r::rs, c::cs -> [r; c] @ mergeRowColumn rs cs

let private mkRows rowValues colValues inflection =
    rowValues |> List.map
        (fun r ->
            colValues |> List.map
                (fun c ->
                    let word = inflection |> List.find (fun i -> fst i = mergeRowColumn r c) |> snd
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
                    let word = axes.values |> List.find (fun (ind, _) -> ind = v) |> snd
                    html $"<th rowspan=\"{span}\">{word}</th>"
                )
        )

let private mkInflectionTable axes inflection =
    let axesSorted = List.sortBy (fun axis -> -List.length axis.values) axes
    let (verAxes, horAxes) = List.foldBack (fun x (l,r) -> x::r, l) axesSorted ([], [])
    let horAxesWithSpans = mkSpans horAxes
    let verAxesWithSpans = mkSpans verAxes
    let colNames (axis, span) = axis.values |> List.map (fun (_, name) -> html $"<th colspan=\"{span}\">{name}</th>")
    let tablePrefix = mkPrefix (mkRowOrColValues verAxes) verAxesWithSpans
    let rows = mkRows (mkRowOrColValues verAxes) (mkRowOrColValues horAxes) inflection
    let tableRows = List.fold2 (fun acc l1 l2 -> acc @ [ html $" <tr>{l1 @ l2}</tr>" ]) [] tablePrefix rows
    [ html
        $"""
            <table class="inflection">
                {horAxesWithSpans |> List.map (fun (axis, span) ->
                    html  $"<tr>
                                <td colspan=\"{List.length verAxes}\">
                                </td>
                                {colNames (axis, span)}
                            </tr>
                        "
                    )}
                {tableRows}
            </table>
        """ ]

let wordInflectionTemplate axes inflection =
    match Seq.toList axes with
    | [] -> [ html $"" ]
    | [axis] ->
        [ html
            $"""
                <table class="inflection">
                    {axis.values |> List.map (fun (v, name) ->
                        html  $"<tr>
                                    <th>{name}</th>
                                    <td>{inflection |> List.find (fun (key, _) -> key = [v]) |> snd}</td>
                                </tr>
                            ")}
                </table>
            """ ]
    | axesList -> mkInflectionTable axesList inflection
