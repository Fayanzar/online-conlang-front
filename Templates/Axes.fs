module OnlineConlangFront.Templates.Axes

open Browser
open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Loading

open Fable.Core

[<LitElement("axis-value")>]
let rec AxisValues () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
            axisValue = Prop.Of((0, ""))
        |}
    )

    let axisValue, setAxisValue = Hook.useState props.axisValue.Value
    let lid = props.language.Value

    html $"""
        <div>
            <input
                value={snd axisValue}
                @keyup={fun (ev : CustomEvent) ->
                    setAxisValue (fst axisValue, ev.target.Value)
                }>
            <button
                @click={fun _ -> Lit.ofPromise(deleteAxisValue lid (fst axisValue), placeholder=loadingTemplate) |> Lit.render root}>
                ❌
            </button>
            <button
                @click={fun _ -> Lit.ofPromise(putAxisValue lid (fst axisValue) (snd axisValue), placeholder=loadingTemplate) |> Lit.render root}>
                Submit
            </button>
        </div>
    """

and [<LitElement("add-axis-value")>] AddAxisValue () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
            aid = Prop.Of(0)
        |}
    )

    let lid = props.language.Value
    let aid = props.aid.Value
    let axisValue, setAxisValue = Hook.useState ""

    html $"""
        <div>
            <input
                value={axisValue}
                @keyup={fun (ev : CustomEvent) -> setAxisValue ev.target.Value}>
            <button
                @click={fun _ -> Lit.ofPromise(postAxisValue lid aid axisValue, placeholder=loadingTemplate) |> Lit.render root}>
                Add value
            </button>
        </div>
    """

and [<LitElement("add-axis")>] AddAxis () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
        |}
    )

    let lid = props.language.Value
    let axis, setAxis = Hook.useState ""

    html $"""
        <div>
            <input
                value={axis}
                @keyup={fun (ev : CustomEvent) -> setAxis ev.target.Value}>
            <button
                @click={fun _ -> Lit.ofPromise(postAxisName lid axis, placeholder=loadingTemplate) |> Lit.render root}>
                Add value
            </button>
        </div>
    """

and [<LitElement("axes-table")>] AxesTable () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
            axes = Prop.Of(([] : AxisForAPI list), attribute="")
        |}
    )

    let lid = props.language.Value
    let axes, setAxes = Hook.useState props.axes.Value

    let setAxis aid axis =
        let newAxes = axes |> List.map (fun axis' ->
            if aid = axis'.id then { id = aid; name = axis; values = axis'.values }
                              else axis'
        )
        setAxes newAxes

    let axisValue av = html $"<axis-value
            language={lid}
            .axisValue={av}>
        </axis-value>"

    html $"""
        <table>
            <tr>
                <th>Id</th>
                <th>Name</th>
                <th>Values</th>
            </tr>
            {axes |> Seq.map (fun axis ->
                html  $"<tr>
                            <td>{axis.id}
                            </td>
                            <td>
                                <input
                                    @keyup={fun (ev : CustomEvent) -> setAxis axis.id ev.target.Value}
                                    value={axis.name}>
                                <button
                                    @click={fun _ -> Lit.ofPromise(putAxisName lid axis.id axis.name, placeholder=loadingTemplate)
                                                                    |> Lit.render root}>
                                    Submit
                                </button>
                            </td>
                            <td>
                                {axis.values |> List.map (fun av ->
                                    axisValue av
                                )}
                                <add-axis-value
                                    language={lid}
                                    aid={axis.id}>
                                </add-axis-value>
                            </td>
                        </tr>"
            )}
        </table>
        <add-axis language={lid}></add-axis>
    """

and [<LitElement("drag-axes")>] DragAxesElement () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
            axes = Prop.Of(([] : AxisForAPI list), attribute="")
        |}
        init.styles <- [
            css $"""
                div {{
                    width: 200px;
                    background: lightgray;
                    margin: 10px;
                    padding: 10px;
                }}
            """
        ]
    )

    let allAxesValues = props.axes.Value |> List.map (fun axis -> axis.name)

    let h = getCookie "horizontal"
    let droppedHorAxes, setDroppedHorAxes =
        match h with
        | None -> Hook.useState ([] : string list)
        | Some horAxes -> horAxes.Split(",")
                          |> Set
                          |> Set.intersect (Set allAxesValues)
                          |> Set.toList
                          |> Hook.useState

    let v = getCookie "vertical"
    let droppedVerAxes, setDroppedVerAxes =
        match v with
        | None -> Hook.useState ([] : string list)
        | Some verAxes -> verAxes.Split(",")
                          |> Seq.toList
                          |> Set
                          |> Set.intersect (Set allAxesValues)
                          |> Set.toList
                          |> Hook.useState

    let undroppedAxes, setUndroppedAxes = List.except (droppedVerAxes @ droppedHorAxes) allAxesValues |> Hook.useState

    let axisFormat = "text/plain"

    html $"""
        {undroppedAxes |> List.map (fun axis ->
            html $"<p
                @dragstart={fun (ev : DragEvent) ->
                    ev.dataTransfer.clearData() |> ignore
                    ev.dataTransfer.setData(axisFormat, axis) |> ignore
                }
                draggable=true>{axis}
            </p>"
        )}
        <div
            @drop={fun (ev : DragEvent) ->
                ev.preventDefault()
                let axis = ev.dataTransfer.getData("text")
                let newDroppedHorAxes = droppedHorAxes @ [axis]
                let newUndroppedAxes = List.except [axis] undroppedAxes
                let newDroppedVerAxes = List.except [axis] droppedVerAxes
                setUndroppedAxes newUndroppedAxes
                setDroppedHorAxes newDroppedHorAxes
                setDroppedVerAxes newDroppedVerAxes
                }
            @dragover={fun (ev : DragEvent) -> ev.preventDefault()}
            >
            <p><strong>Horizontal axes</strong></p>
            {droppedHorAxes |> List.map (fun axis ->
                html $"<p
                    @dragstart={fun (ev : DragEvent) ->
                        ev.dataTransfer.clearData() |> ignore
                        ev.dataTransfer.setData(axisFormat, axis) |> ignore
                    }
                    draggable=true>{axis}
                </p>"
            )}
        </div>
        <div
            @drop={fun (ev : DragEvent) ->
                ev.preventDefault()
                let axis = ev.dataTransfer.getData("text")
                let newDroppedHorAxes = List.except [axis] droppedHorAxes
                let newUndroppedAxes = List.except [axis] undroppedAxes
                let newDroppedVerAxes = droppedVerAxes @ [axis]
                setUndroppedAxes newUndroppedAxes
                setDroppedHorAxes newDroppedHorAxes
                setDroppedVerAxes newDroppedVerAxes
                }
            @dragover={fun (ev : DragEvent) -> ev.preventDefault()}
            >
            <p><strong>Vertical axes</strong></p>
            {droppedVerAxes |> List.map (fun axis ->
                html $"<p
                    @dragstart={fun (ev : DragEvent) ->
                        ev.dataTransfer.clearData() |> ignore
                        ev.dataTransfer.setData(axisFormat, axis) |> ignore
                    }
                    draggable=true>{axis}
                </p>"
            )}
        </div>
        <button
            @click={fun _ ->
                setCookie "horizontal" (droppedHorAxes |> String.concat ",") |> ignore
                setCookie "vertical" (droppedVerAxes |> String.concat ",") |> ignore
            }>
            Save preferences
        </button>
    """

and axesTemplate lid =
    promise {
        let! axes = server.getAxes lid |> Async.StartAsPromise
        return
            html $"""
                <axes-table
                    language={lid}
                    .axes={axes}>
                </axes-table>
                <drag-axes
                    language={lid}
                    .axes={axes}>
                </drag-axes>
            """
    }

and postAxisValue lid aid v =
    promise {
        do! server.postAxisValue aid v |> Async.StartAsPromise
        return! axesTemplate lid
    }

and deleteAxisValue lid vid =
    promise {
        do! server.deleteAxisValue vid |> Async.StartAsPromise
        return! axesTemplate lid
    }

and putAxisValue lid avid v =
    promise {
        do! server.putAxisValue avid v |> Async.StartAsPromise
        return! axesTemplate lid
    }

and putAxisName lid aid axis =
    promise {
        do! server.putAxisName aid axis |> Async.StartAsPromise
        return! axesTemplate lid
    }

and postAxisName lid axis =
    promise {
        do! server.postAxisName lid axis |> Async.StartAsPromise
        return! axesTemplate lid
    }
