module OnlineConlangFront.Templates.Rules

open Fable.Core
open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation

open OnlineConlangFront.Import.Inflection
open OnlineConlangFront.Templates.Loading
open OnlineConlangFront.Templates.InflectionRules

open System

let rulePresentAbsentClasses rule =
    match rule with
    | TRule _ -> ("transformation", [| "infix"; "prefix"; "suffix" |])
    | AffixRule ar ->
        match ar with
        | Prefix _ -> ("prefix", [| "infix"; "suffix"; "transformation" |])
        | Suffix _ -> ("suffix", [| "infix"; "prefix"; "transformation" |])
        | Infix  _ -> ("infix", [| "suffix"; "prefix"; "transformation" |])

let changeBackground (ev : CustomEvent) =
    let el = (ev.target :?> HTMLElement).parentElement
    printfn "%A" el.nodeName
    let classes = rulePresentAbsentClasses (ev.detail :?> Rule)
    el.classList.remove (snd classes)
    el.classList.add (fst classes)

let space = " "

let rec postAxisRules lid iid avid newRules =
    promise {
        do! server.postAxisRules getJWTCookie iid avid newRules |> Async.StartAsPromise
        return! rulesTemplate lid
    }

and [<LitElement("axis-rules")>] AxisRules () =
    let _, props =
        LitElement.init (fun init ->
            init.styles <- [
                yield! OnlineConlangFront.Shared.styles

                css $"""
                    div.axis-rules {{
                        padding: 5px 5px 5px 5px;
                    }}

                    div.rule-container {{
                        padding: 5px 5px 5px 5px;
                    }}

                    .rule-parent.dragover {{
                        border-top: 2px dashed white;
                    }}

                    .rule-container.dragover {{
                        border-bottom: 2px dashed white;
                    }}
                """
            ]
            init.props <- {|
                language = Prop.Of(0)
                inflection = Prop.Of(emptyInflection, attribute="")
                rules = Prop.Of((0, ([] : (int * Rule) list)), attribute = "")
                axesNames = Prop.Of(([] : (int * string) list), attribute = "")
            |})

    let lid = props.language.Value
    let iid = props.inflection.Value.id
    let uuid = Guid.NewGuid()

    let rules, setRules = Hook.useState props.rules.Value
    let findAxisValueName avid =
        props.axesNames.Value |> List.tryFind (fun (k, _) -> k = avid)
                              |> Option.map snd |> Option.defaultValue ""

    let replaceRule (i, r) =
        let newRules = snd rules |> List.map (fun (i', r') -> if i = i' then (i, r) else (i', r'))
        setRules (fst rules, newRules)

    let deleteRule i =
        let newRules = snd rules |> List.filter (fun (i', _) -> i' <> i)
        setRules (fst rules, newRules)

    let onDragStart i (ev : DragEvent) =
        ev.dataTransfer.clearData() |> ignore
        ev.dataTransfer.setData("text/plain", $"{uuid},{i}")

    let validateData (ev : DragEvent) =
        let data = ev.dataTransfer.getData("text").Split(",")
        match data with
        | [| uuid'; ind' |] when uuid' = uuid.ToString() -> Some ind'
        | _ -> None

    let onDrop i (ev : DragEvent) =
        match validateData ev with
        | Some ind' ->
            ev.preventDefault()
            let ruleI = ind' |> int
            let orule = List.tryFind (fun (i', _) -> i' = ruleI) (snd rules)
            match orule with
            | None -> ()
            | Some rule ->
                let filteredRules = List.except [rule] (snd rules)
                let oruleIndex = List.tryFindIndex (fun (i', _) -> i' = i) filteredRules
                match oruleIndex with
                | None -> ()
                | Some ruleIndex ->
                    let newRules = List.insertAt ruleIndex rule filteredRules
                    setRules (fst rules, newRules)
        | _ -> ()

    let ruleParentClass = "rule-parent"
    let rootName = "axis-rules"
    let moveEffect = "move"
    let dragoverClass = "dragover"

    let rec removeClassAllParents (el : HTMLElement) cl =
        el.classList.remove cl
        match el.parentElement with
        | null -> ()
        | parent -> removeClassAllParents parent cl

    let onDragEnter (ev : DragEvent) =
        match validateData ev with
        | Some _ ->
            let el = ev.target :?> HTMLElement
            if el.classList.contains "rule-container" || el.classList.contains "rule-parent" then
                ev.stopPropagation()
                el.classList.add dragoverClass
        | _ -> ()

    let onDragLeave isDrop (ev : DragEvent) =
        match validateData ev with
        | Some _ ->
            let el = ev.target :?> HTMLElement
            let rect = el.getBoundingClientRect()
            let isInside = ev.clientX < rect.right
                           && ev.clientX > rect.left
                           && ev.clientY < rect.bottom
                           && ev.clientY > rect.top
            if isDrop then
                removeClassAllParents el [| dragoverClass |]
            else
                if el.classList.contains "rule-parent" && (not isInside) then
                    ev.stopPropagation()
                    el.classList.remove dragoverClass
                if el.classList.contains "rule-container" then
                    ev.stopPropagation()
                    el.classList.remove dragoverClass
        | _ -> ()

    let onDropLast (ev : DragEvent) =
        match validateData ev with
        | Some ind' ->
            ev.preventDefault()
            let ruleI = ind' |> int
            let rule = List.tryFind (fun (i', _) -> i' = ruleI) (snd rules)
                    |> Option.defaultValue (0, TRule emptyTransformation)
            let filteredRules = List.except [rule] (snd rules)
            let newRules = filteredRules @ [rule]
            setRules (fst rules, newRules)
        | _ -> ()

    html
        $"""
        <div class=axis-rules>
            <div class=rule-container id={uuid}
                @dragover={fun (ev : DragEvent) ->
                    match validateData ev with
                    | Some _ -> ev.preventDefault()
                    | _ -> ()
                }
                @dragenter={onDragEnter}
                @dragleave={onDragLeave false}
                @drop={fun (ev : DragEvent) ->
                    onDragLeave true ev
                    onDropLast ev
                }>
                <p style="text-align: center;">{findAxisValueName <| fst rules}</p>
                {LitBindings.repeat (snd rules,
                                    (fun r -> $"{fst r}"),
                                    (fun r _ -> html $"<div @rule-changed={fun (ev : CustomEvent) ->
                                                            let rule = ev.detail :?> Rule
                                                            replaceRule (fst r, rule)
                                                        }
                                                        @rule-type-changed={changeBackground}
                                                        class={
                                                                let classes = rulePresentAbsentClasses (snd r)
                                                                [fst classes; ruleParentClass] |> String.concat space
                                                        }
                                                        @dragstart={fun (ev : DragEvent) ->
                                                            ev.stopPropagation()
                                                            ev.dataTransfer.effectAllowed <- moveEffect
                                                            onDragStart (fst r) ev
                                                        }
                                                        @drop={fun (ev : DragEvent) ->
                                                            onDragLeave true ev
                                                            onDrop (fst r) ev
                                                        }
                                                        @dragover={fun (ev : DragEvent) ->
                                                            match validateData ev with
                                                            | Some _ -> ev.preventDefault()
                                                            | _ -> ()
                                                        }
                                                        @dragenter={onDragEnter}
                                                        @dragleave={onDragLeave false}
                                                        @rule-deleted={(fun _ -> deleteRule (fst r))}
                                                        draggable=true>
                                                            {ruleElement r rootName}
                                                    </div>"))
                }
            </div>
            <button
                @click={Ev(fun _ ->
                    let nextInd = snd rules |> List.map fst |> function
                                    | [] -> 0
                                    | l -> 1 + List.max l
                    let newRule = (nextInd, TRule emptyTransformation)
                    setRules (fst rules, (snd rules) @ [newRule])
                )}
            >Add item</button>
            <button
                @click={Ev(fun _ ->
                    let avid = fst rules
                    let newRules = snd rules |> List.map snd
                    Lit.ofPromise(postAxisRules  lid iid avid newRules, placeholder=loadingTemplate) |> Lit.render root
                )}
            >Submit
            </button>
        </div>
        """

and postOverrideRules lid iid newOverrideRules =
    promise {
        do! server.postOverrideRules getJWTCookie iid newOverrideRules |> Async.StartAsPromise
        return! rulesTemplate lid
    }

and [<LitElement("axes-select")>] AxesSelect () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- OnlineConlangFront.Shared.styles
            init.props <- {|
                inflectionAxes = Prop.Of(([] : ((int * string) list) list), attribute = "")
                selectedAxes = Prop.Of(([] : (int * string) list), attribute = "")
            |})

    let selectedAxes, setSelectedAxes = Hook.useState props.selectedAxes.Value
    let inflectionAxes = props.inflectionAxes.Value

    let optionAxis i (v, axis) =
        let selectedAxis = List.tryItem i selectedAxes
        if Option.map fst selectedAxis = Some v then
            html $"<option selected>{axis}</option>"
        else
            html $"<option>{axis}</option>"

    let replaceAxis v newAxis =
        let newSelectedAxes = selectedAxes |> List.mapi (fun i axis ->
            if i = v then
                let inflectionAxis = inflectionAxes |> List.tryItem v |> Option.defaultValue []
                let newAxisValue = inflectionAxis
                                   |> List.tryFind (fun (_, a') -> a' = newAxis)
                                   |> Option.map fst
                                   |> Option.defaultValue 0
                (newAxisValue, newAxis)
            else axis
        )
        setSelectedAxes newSelectedAxes
        host.dispatchCustomEvent("axes-changed", newSelectedAxes |> List.map fst)

    let selectStyle = "display: flex;"

    html $"""
        {inflectionAxes |> List.mapi (fun i axes ->
            html  $"<div class=select style={selectStyle}>
                        <select
                            @change={fun (ev : Event) ->
                                let newAxis = ev.target.Value
                                replaceAxis i newAxis
                            }>
                            {axes |> List.map (fun axis -> optionAxis i axis)}
                        </select>
                    </div>"
        )}
    """

and [<LitElement("override-rules")>] OverrideRules () =
    let _, props =
        LitElement.init (fun init ->
            init.styles <- [
                yield! OnlineConlangFront.Shared.styles

                css $"""
                    div.rule-container {{
                        padding: 5px 5px 5px 5px;
                    }}

                    .rule-parent.dragover {{
                        border-top: 2px dashed white;
                    }}

                    .rule-container.dragover {{
                        border-bottom: 2px dashed white;
                    }}

                    .grid, .grid tr, .grid th, .grid td {{
                        background-color: inherit !important;
                        border: none;
                    }}
                """
            ]
            init.props <- {|
                language = Prop.Of(0)
                overrideRules = Prop.Of(([] : list<list<int> * list<int * Rule>>), attribute = "")
                axesNames = Prop.Of(([] : (int * string) list), attribute = "")
                inflection = Prop.Of(emptyInflection, attribute="")
            |})

    let lid = props.language.Value
    let iid = props.inflection.Value.id

    let inflectionAxesValues = props.inflection.Value.axes.axes |> List.map (fun a ->
        a.inflections |> Map.keys |> Seq.toList)

    let findAxisValueName avid =
        props.axesNames.Value |> List.tryFind (fun (k, _) -> k = avid)
                              |> Option.map snd |> Option.defaultValue ""

    let axesCombined = inflectionAxesValues |> List.map (List.map (fun v -> (v, findAxisValueName v)))

    let overrideRules, setOverrideRules = Hook.useState props.overrideRules.Value

    let replaceOverrideRule axes (i, r) =
        let newRules = overrideRules |> List.map (fun (axes', rules') ->
            if Set axes' = Set axes then
                (axes', rules' |> List.map (fun (i', r') -> if i = i' then (i, r) else (i', r')))
            else
                (axes', rules')
        )
        setOverrideRules newRules

    let addRule axes newRule =
        let newRules = overrideRules |> List.map (fun (axes', rules') ->
            if Set axes' = Set axes then (axes', rules' @ [newRule])
                            else (axes', rules')
        )
        setOverrideRules newRules

    let deleteRule axes i =
        let newRules = overrideRules |> List.map (fun (axes', rules') ->
            if Set axes' = Set axes then (axes', rules' |> List.filter (fun (i', _) -> i' <> i))
            else (axes', rules'))
        setOverrideRules newRules

    let deleteAxes axes =
        let newRules = overrideRules |> List.filter (fun (axes', _) -> Set axes' <> Set axes)
        setOverrideRules newRules

    let replaceAxes axes newAxes =
        let newRules = overrideRules |> List.map (fun (axes', rules') ->
            if Set axes = Set axes' then (newAxes, rules')
                                    else (axes', rules'))
        setOverrideRules newRules

    let addAxes () =
        let newAxes = cartesianN inflectionAxesValues |> List.head
        setOverrideRules (overrideRules @ [(newAxes, [])])

    let rec ungroupList (l : ('a * 'b list) list) =
        match l with
        | [] -> []
        | (k, v)::xs ->
            match v with
            | [] -> ungroupList xs
            | y::ys -> (k, y)::ungroupList ((k, ys)::xs)


    let validateData (ev : DragEvent) uuid =
        let data = ev.dataTransfer.getData("text").Split(",")
        match data with
        | [| uuid'; ind' |] when uuid' = uuid.ToString() -> Some ind'
        | _ -> None

    let onDragStart i uuid (ev : DragEvent) =
        ev.dataTransfer.clearData() |> ignore
        ev.dataTransfer.setData("text/plain", $"{uuid},{i}")

    let onDrop axes i (ev : DragEvent) uuid =
        match validateData ev uuid with
        | Some ind' ->
            ev.preventDefault()
            let ruleI = ind' |> int
            let newRules = overrideRules |> List.map (fun (axes', rules') ->
                if axes' = axes then
                    let orule = List.tryFind (fun (i, _) -> i = ruleI) rules'
                    match orule with
                    | None -> (axes', rules')
                    | Some rule ->
                        let filteredRules = List.except [rule] rules'
                        let oruleIndex = List.tryFindIndex (fun (i', _) -> i' = i) filteredRules
                        match oruleIndex with
                        | None -> (axes', rules')
                        |Some ruleIndex ->
                            let newRules = List.insertAt ruleIndex rule filteredRules
                            (axes', newRules)
                else (axes', rules')
            )
            setOverrideRules newRules
        | _ -> ()

    let ruleParentClass = "rule-parent"
    let moveEffect = "move"
    let rootName = "override-rules"
    let dragoverClass = "dragover"

    let rec removeClassAllParents (el : HTMLElement) cl =
        el.classList.remove cl
        match el.parentElement with
        | null -> ()
        | parent -> removeClassAllParents parent cl

    let onDragEnter uuid (ev : DragEvent) =
        match validateData ev uuid with
        | Some _ ->
            let el = ev.target :?> HTMLElement
            if el.classList.contains "rule-container" || el.classList.contains "rule-parent" then
                ev.stopPropagation()
                el.classList.add dragoverClass
        | _ -> ()

    let onDragLeave uuid isDrop (ev : DragEvent) =
        match validateData ev uuid with
        | Some _ ->
            let el = ev.target :?> HTMLElement
            let rect = el.getBoundingClientRect()
            let isInside = ev.clientX < rect.right
                           && ev.clientX > rect.left
                           && ev.clientY < rect.bottom
                           && ev.clientY > rect.top
            if isDrop then
                removeClassAllParents el [| dragoverClass |]
            else
                if el.classList.contains "rule-parent" && (not isInside) then
                    ev.stopPropagation()
                    el.classList.remove dragoverClass
                if el.classList.contains "rule-container" then
                    ev.stopPropagation()
                    el.classList.remove dragoverClass
        | _ -> ()

    let onDropLast axes (ev : DragEvent) uuid =
        match validateData ev uuid with
        | Some ind' ->
            ev.preventDefault()
            let ruleI = ind' |> int
            let newRules = overrideRules |> List.map (fun (axes', rules') ->
                if axes' = axes then
                    let rule = List.tryFind (fun (i, _) -> i = ruleI) rules'
                            |> Option.defaultValue (0, TRule emptyTransformation)
                    let filteredRules = List.except [rule] rules'
                    let newRules = filteredRules @ [rule]
                    (axes', newRules)
                else (axes', rules')
            )
            setOverrideRules newRules
        | _ -> ()

    let rulesTemplate axes rules uuid =
        html $"""
            <div class=rule-container id={uuid}
                @dragover={fun (ev : DragEvent) ->
                    match validateData ev uuid with
                    | Some _ -> ev.preventDefault()
                    | _ -> ()
                }
                @drop={fun (ev : DragEvent) ->
                    onDragLeave uuid true ev
                    onDropLast axes ev uuid
                }
                @dragenter={onDragEnter uuid}
                @dragleave={onDragLeave uuid false}>
            {LitBindings.repeat (rules,
                                (fun r -> $"{fst r}"),
                                (fun r _ -> html $"<div @rule-changed={(fun (ev : CustomEvent) ->
                                                            let a = ev.detail :?> Rule
                                                            replaceOverrideRule axes (fst r, a))
                                                        }
                                                        @rule-deleted={fun _ -> deleteRule axes (fst r)}
                                                        @rule-type-changed={changeBackground}
                                                        class={
                                                                let classes = rulePresentAbsentClasses (snd r)
                                                                [fst classes; ruleParentClass] |> String.concat space
                                                        }
                                                        draggable=true
                                                        @dragstart={fun (ev : DragEvent) ->
                                                            ev.stopPropagation()
                                                            ev.dataTransfer.effectAllowed <- moveEffect
                                                            onDragStart (fst r) uuid ev
                                                        }
                                                        @dragover={fun (ev : DragEvent) ->
                                                            match validateData ev uuid with
                                                            | Some _ -> ev.preventDefault()
                                                            | _ -> ()
                                                        }
                                                        @dragenter={onDragEnter uuid}
                                                        @dragleave={onDragLeave uuid false}
                                                        @drop={fun (ev : DragEvent) ->
                                                            onDragLeave uuid true ev
                                                            onDrop axes (fst r) ev uuid
                                                        }>
                                                        {ruleElement r rootName}
                                                    </div>"))
            }
            </div>
            <button
                @click={Ev(fun _ ->
                    let nextInd = rules |> List.map fst |> function
                                    | [] -> 0
                                    | l -> 1 + List.max l
                    let newRule = (nextInd, TRule emptyTransformation)
                    addRule axes newRule
                )}
            >Add item</button>
        """

    html $"""
        <table>
            <tr>
                <th>Axes</th>
                <th>Rule</th>
            </tr>
            {LitBindings.repeat (overrideRules,
                                (fun (axes, _) -> axes |> List.map (fun a -> a.ToString()) |> String.concat ""),
                                fun (axes, rules) _ ->
                                    let axesNames = axes |> List.map findAxisValueName |> List.distinct
                                    let uuid = Guid.NewGuid()
                                    html  $"<tr>
                                                <td>
                                                    <table class=grid>
                                                        <tr>
                                                            <td>
                                                                <button
                                                                    @click={fun _ -> deleteAxes axes}>
                                                                    ❌
                                                                </button>
                                                            </td>
                                                            <td>
                                                                <axes-select
                                                                    @axes-changed={fun (ev : CustomEvent) ->
                                                                        let newAxes = ev.detail :?> int list
                                                                        replaceAxes axes newAxes
                                                                    }
                                                                    .inflectionAxes={axesCombined}
                                                                    .selectedAxes={List.zip axes axesNames}>
                                                                </axes-select>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>
                                                    {rulesTemplate axes rules uuid}
                                                </td>
                                            </tr>"
                                )}
        </table>
        <div style="position: sticky; bottom: 20px;">
            <button
                @click={fun _ -> addAxes ()}>
                Add axes
            </button>
            <button
                @click={fun _ ->
                    let rulesNoInd = overrideRules |> List.map (fun (axes, rules) -> (axes, List.map snd rules))
                    let ungroupedRules = ungroupList rulesNoInd |> List.map (fun (axes, r) ->
                        { overrideRule = r; overrideAxes = axes })
                    Lit.ofPromise (postOverrideRules lid iid ungroupedRules, placeholder=loadingTemplate) |> Lit.render root
                    }>
                Submit
            </button>
        </div>
    """

and rulesTemplate lid  =
    promise {
        let! inflections = server.getInflections lid |> Async.StartAsPromise
        let! axes = server.getAxes lid |> Async.StartAsPromise
        let axesTable (axis : Axis) (inflection : InflectTForAPI) =
            html
                $"""
                    <table class=inflections>
                        <tr>
                            <th>{axis.name}</th>
                        </tr>
                        {axis.inflections |> Map.map (
                            fun k v -> html $"<tr><td><axis-rules
                                                language={lid}
                                                .inflection={inflection}
                                                .rules={(k, List.mapi (fun i r -> (i, r)) v)}
                                                .axesNames = {axes |> Seq.map (fun a -> a.values) |> Seq.concat |> Seq.toList}>
                                                </axis-rules></td></tr>"
                            ) |> Map.toList |> List.map snd}
                    </table>
                """
        let classP cl = html $"<p>{cl}</p>"

        return html
            $"""
                <table>
                    <tr>
                        <th>Part of speech</th>
                        <th>Classes</th>
                        <th>Axes</th>
                        <th>Overrides</th>
                    </tr>
                    {inflections |> Seq.map (fun inflection ->
                        let overrideRules = inflection.axes.overrides |> List.groupBy fst
                                            |> List.map (fun (k, v) ->
                                            (k, v |> List.map snd |> List.concat |> List.mapi (fun i r -> (i, r))))
                        html  $"
                        <tr>
                            <td>{inflection.speechPart}</td>
                            <td>{inflection.classes |> Set.toList |> List.map (fun cl ->
                                    classP cl
                                )}</td>
                            <td>{inflection.axes.axes |> List.map (fun a -> axesTable a inflection)}</td>
                            <td>
                                <override-rules
                                    language={lid}
                                    .axesNames={axes |> Seq.map (fun a -> a.values) |> Seq.concat |> Seq.toList}
                                    .overrideRules={overrideRules}
                                    .inflection={inflection}>
                                </override-rules>
                            </td>
                        </tr>
                        "
                    )}
                </table>
            """
    }
