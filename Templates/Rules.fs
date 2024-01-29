module OnlineConlangFront.Templates.Rules

open Fable.Core
open Browser
open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation

open OnlineConlangFront.Import.Inflection
open OnlineConlangFront.Templates.Loading

let emptyTransformation = { input = ""; output = ""; applyMultiple = false }

let emptyInflection = { language = 0
                      ; speechPart = ""
                      ; classes = Set.empty
                      ; axes = { axes = []; overrides = [] }
                      ; id = 0
                      ; name = None
                      }

[<LitElement("suffix-rule")>]
let SuffixRule () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- [
                css $"""
                    input:not([type='checkbox']) {{
                        width: 50px;
                    }}
                """
            ]
            init.props <- {|
                rootName = Prop.Of("")
                suffix = Prop.Of("")
                transformation = Prop.Of(emptyTransformation, attribute = "")
            |})

    let suffix, setSuffix = Hook.useState props.suffix.Value
    let suffixRef = Hook.useRef<HTMLInputElement>()
    let input, setInput = Hook.useState props.transformation.Value.input
    let inputRef = Hook.useRef<HTMLInputElement>()
    let output, setOutput = Hook.useState props.transformation.Value.output
    let outputRef = Hook.useRef<HTMLInputElement>()
    let applyMultipleRef = Hook.useRef<HTMLInputElement>()

    let rootName = props.rootName.Value

    let rootElements =
        match rootName with
        | "" -> []
        | _ ->
            let elements = document.querySelectorAll(rootName)
            seq { for i in 0..(elements.length-1) -> elements.Item i } |> Seq.toList

    let removeDraggable () =
        for el in rootElements do
            let ruleParents = el.shadowRoot.querySelectorAll "div.rule-parent"
            for i in 0..(ruleParents.length - 1) do
                let parent = ruleParents.Item i
                parent.setAttribute ("draggable", "false")

    let addDraggable () =
        for el in rootElements do
            let ruleParents = el.shadowRoot.querySelectorAll "div.rule-parent"
            for i in 0..(ruleParents.length - 1) do
                let parent = ruleParents.Item i
                parent.setAttribute ("draggable", "true")

    let onRuleChanged () =
        let newInput = inputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newOutput = outputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newApplyMultiple = applyMultipleRef.Value |> Option.map (fun v -> v.checked) |> Option.defaultValue false
        let newSuffix = suffixRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newT = { input = newInput; output = newOutput; applyMultiple = newApplyMultiple }
        let newR = AffixRule (Suffix (newSuffix, newT))
        host.dispatchCustomEvent ("rule-changed", newR)

    html
        $"""
            word-
            <input {Lit.refValue suffixRef}
                value={suffix}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (e : Event) ->
                    setSuffix e.target.Value
                    onRuleChanged ()
                }>
            ;
            <input {Lit.refValue inputRef}
                value={input}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (e : Event) ->
                    setInput e.target.Value
                    onRuleChanged ()
                }>
            →
            <input {Lit.refValue outputRef}
                value={output}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (e : Event) ->
                    setOutput e.target.Value
                    onRuleChanged ()
                }>
            <label>w₁→w₂→…→wₙ</label>
            <input {Lit.refValue applyMultipleRef}
                type="checkbox"
                ?checked={props.transformation.Value.applyMultiple}>
            <button
                @click={fun _ -> host.dispatchEvent ("rule-deleted")}>
                ❌
            </button>
        """

[<LitElement("prefix-rule")>]
let PrefixRule () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- [
                css $"""
                    input:not([type='checkbox']) {{
                        width: 50px;
                    }}
                """
            ]
            init.props <- {|
                rootName = Prop.Of("")
                prefix = Prop.Of("")
                transformation = Prop.Of(emptyTransformation, attribute = "")
            |})

    let prefix, setPrefix = Hook.useState props.prefix.Value
    let prefixRef = Hook.useRef<HTMLInputElement>()
    let input, setInput = Hook.useState props.transformation.Value.input
    let inputRef = Hook.useRef<HTMLInputElement>()
    let output, setOutput = Hook.useState props.transformation.Value.output
    let outputRef = Hook.useRef<HTMLInputElement>()
    let applyMultipleRef = Hook.useRef<HTMLInputElement>()

    let rootName = props.rootName.Value

    let rootElements =
        match rootName with
        | "" -> []
        | _ ->
            let elements = document.querySelectorAll(rootName)
            seq { for i in 0..(elements.length-1) -> elements.Item i } |> Seq.toList

    let removeDraggable () =
        for el in rootElements do
            let ruleParents = el.shadowRoot.querySelectorAll "div.rule-parent"
            for i in 0..(ruleParents.length - 1) do
                let parent = ruleParents.Item i
                parent.setAttribute ("draggable", "false")

    let addDraggable () =
        for el in rootElements do
            let ruleParents = el.shadowRoot.querySelectorAll "div.rule-parent"
            for i in 0..(ruleParents.length - 1) do
                let parent = ruleParents.Item i
                parent.setAttribute ("draggable", "true")

    let onRuleChanged () =
        let newInput = inputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newOutput = outputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newApplyMultiple = applyMultipleRef.Value |> Option.map (fun v -> v.checked) |> Option.defaultValue false
        let newPrefix = prefixRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newT = { input = newInput; output = newOutput; applyMultiple = newApplyMultiple }
        let newR = AffixRule (Prefix (newPrefix, newT))
        host.dispatchCustomEvent ("rule-changed", newR)
    html
        $"""
            <input {Lit.refValue prefixRef}
                value={prefix}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (e : Event) ->
                    setPrefix e.target.Value
                    onRuleChanged ()
                }>
            -word;
            <input {Lit.refValue inputRef}
                value={input}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (e : Event) ->
                    setInput e.target.Value
                    onRuleChanged ()
                }>
            →
            <input {Lit.refValue outputRef}
                value={output}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (e : Event) ->
                    setOutput e.target.Value
                    onRuleChanged ()
                }>

            <label>w₁→w₂→…→wₙ</label>
            <input {Lit.refValue applyMultipleRef}
                type="checkbox"
                ?checked={props.transformation.Value.applyMultiple}>
            <button
                @click={fun _ -> host.dispatchEvent ("rule-deleted")}>
                ❌
            </button>
        """

[<LitElement("infix-rule")>]
let InfixRule () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- [
                css $"""
                    input:not([type='checkbox']) {{
                        width: 50px;
                    }}
                """
            ]
            init.props <- {|
                rootName = Prop.Of("")
                infix = Prop.Of("")
                transformation1 = Prop.Of(emptyTransformation, attribute = "")
                transformation2 = Prop.Of(emptyTransformation, attribute = "")
                position = Prop.Of(0)
            |})

    let infix, setInfix = Hook.useState props.infix.Value
    let infixRef = Hook.useRef<HTMLInputElement>()
    let position, setPosition = Hook.useState props.position.Value
    let positionRef = Hook.useRef<HTMLInputElement>()
    let input1, setInput1 = Hook.useState props.transformation1.Value.input
    let inputRef1 = Hook.useRef<HTMLInputElement>()
    let output1, setOutput1 = Hook.useState props.transformation1.Value.output
    let outputRef1 = Hook.useRef<HTMLInputElement>()
    let input2, setInput2 = Hook.useState props.transformation2.Value.input
    let inputRef2 = Hook.useRef<HTMLInputElement>()
    let output2, setOutput2 = Hook.useState props.transformation2.Value.output
    let outputRef2 = Hook.useRef<HTMLInputElement>()
    let applyMultipleRef1 = Hook.useRef<HTMLInputElement>()
    let applyMultipleRef2 = Hook.useRef<HTMLInputElement>()

    let rootName = props.rootName.Value

    let rootElements =
        match rootName with
        | "" -> []
        | _ ->
            let elements = document.querySelectorAll(rootName)
            seq { for i in 0..(elements.length-1) -> elements.Item i } |> Seq.toList

    let removeDraggable () =
        for el in rootElements do
            let ruleParents = el.shadowRoot.querySelectorAll "div.rule-parent"
            for i in 0..(ruleParents.length - 1) do
                let parent = ruleParents.Item i
                parent.setAttribute ("draggable", "false")

    let addDraggable () =
        for el in rootElements do
            let ruleParents = el.shadowRoot.querySelectorAll "div.rule-parent"
            for i in 0..(ruleParents.length - 1) do
                let parent = ruleParents.Item i
                parent.setAttribute ("draggable", "true")

    let onRuleChanged () =
        let newInput1 = inputRef1.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newInput2 = inputRef2.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newOutput1 = outputRef1.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newOutput2 = outputRef1.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newApplyMultiple1 = applyMultipleRef1.Value |> Option.map (fun v -> v.checked) |> Option.defaultValue false
        let newApplyMultiple2 = applyMultipleRef2.Value |> Option.map (fun v -> v.checked) |> Option.defaultValue false
        let newInfix = infixRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newPosition = positionRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue "" |> int
        let newT1 = { input = newInput1; output = newOutput1; applyMultiple = newApplyMultiple1 }
        let newT2 = { input = newInput2; output = newOutput2; applyMultiple = newApplyMultiple2 }
        let newR = AffixRule (Infix (newInfix, newT1, newT2, newPosition))
        host.dispatchCustomEvent ("rule-changed", newR)

    html
        $"""
            <input {Lit.refValue inputRef1}
                value={input1}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (ev : Event) ->
                    setInput1 ev.target.Value
                    onRuleChanged ()
                }>
            →
            <input {Lit.refValue outputRef1}
                value={output1}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (ev : Event) ->
                    setOutput1 ev.target.Value
                    onRuleChanged ()
                }>

            <label>w₁→w₂→…→wₙ</label>
            <input {Lit.refValue applyMultipleRef1}
                type="checkbox"
                ?checked={props.transformation1.Value.applyMultiple}
                @click={fun _ -> onRuleChanged ()}>

            ; w<
            <input {Lit.refValue infixRef}
                value={infix}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (ev : Event) ->
                    setInfix ev.target.Value
                    onRuleChanged ()
                }>

            <label>ᵢ</label>
            <input {Lit.refValue positionRef} type=number
                value={position}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (ev : Event) ->
                    ev.target.Value |> int |> setPosition
                    onRuleChanged ()
                }>
            >ord;

            <input {Lit.refValue inputRef2}
                value={input2}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (ev : Event) ->
                    setInput2 ev.target.Value
                    onRuleChanged ()
                }>
            →
            <input {Lit.refValue outputRef2}
                value={output2}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (ev : Event) ->
                    setOutput2 ev.target.Value
                    onRuleChanged ()
                }>

            <label>w₁→w₂→…→wₙ</label>
            <input {Lit.refValue applyMultipleRef2}
                type="checkbox"
                ?checked={props.transformation2.Value.applyMultiple}
                @click={fun _ -> onRuleChanged ()}>
            <button
                @click={fun _ -> host.dispatchEvent ("rule-deleted")}>
                ❌
            </button>
        """

[<LitElement("transform-rule")>]
let TransformRule () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- [
                css $"""
                    input:not([type='checkbox']) {{
                        width: 50px;
                    }}
                """
            ]
            init.props <- {|
                rootName = Prop.Of("")
                transformation = Prop.Of(emptyTransformation, attribute = "")
            |})

    let input, setInput = Hook.useState props.transformation.Value.input
    let inputRef = Hook.useRef<HTMLInputElement>()
    let output, setOutput = Hook.useState props.transformation.Value.output
    let outputRef = Hook.useRef<HTMLInputElement>()
    let applyMultipleRef = Hook.useRef<HTMLInputElement>()

    let rootName = props.rootName.Value

    let rootElements =
        match rootName with
        | "" -> []
        | _ ->
            let elements = document.querySelectorAll(rootName)
            seq { for i in 0..(elements.length-1) -> elements.Item i } |> Seq.toList

    let removeDraggable () =
        for el in rootElements do
            let ruleParents = el.shadowRoot.querySelectorAll "div.rule-parent"
            for i in 0..(ruleParents.length - 1) do
                let parent = ruleParents.Item i
                parent.setAttribute ("draggable", "false")

    let addDraggable () =
        for el in rootElements do
            let ruleParents = el.shadowRoot.querySelectorAll "div.rule-parent"
            for i in 0..(ruleParents.length - 1) do
                let parent = ruleParents.Item i
                parent.setAttribute ("draggable", "true")

    let onRuleChanged () =
        let newInput = inputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newOutput = outputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
        let newApplyMultiple = applyMultipleRef.Value |> Option.map (fun v -> v.checked) |> Option.defaultValue false
        let newT = { input = newInput; output = newOutput; applyMultiple = newApplyMultiple}
        host.dispatchCustomEvent ("rule-changed", TRule newT)

    html
        $"""
            <input {Lit.refValue inputRef}
                value={input}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (ev : Event) ->
                    setInput (ev.target.Value)
                    onRuleChanged ()
                }>
            →
            <input {Lit.refValue outputRef}
                value={output}
                @mousedown={fun (e : Event) ->
                    e.stopPropagation()
                    removeDraggable()
                }
                @mouseup={fun _ ->
                    addDraggable()
                }
                @keyup={fun (ev : Event) ->
                    setOutput (ev.target.Value)
                    onRuleChanged ()
                }>

            <label>w₁→w₂→…→wₙ</label>
            <input {Lit.refValue applyMultipleRef}
                type="checkbox"
                ?checked={props.transformation.Value.applyMultiple}
                @click={fun _ -> onRuleChanged ()}>

            <button
                @click={fun _ -> host.dispatchEvent ("rule-deleted")}>
                ❌
            </button>
        """

let ruleTemplate rule rootName =
    match rule with
    | TRule t ->
        html $"<transform-rule .transformation={t} rootName={rootName}></transform-rule>"
    | AffixRule a ->
        match a with
        | Suffix (s, t) ->
            html $"""
                <suffix-rule suffix={s} .transformation={t} rootName={rootName}></suffix-rule>
            """
        | Prefix (s, t) ->
            html $"""
                <prefix-rule prefix={s} .transformation={t} rootName={rootName}></prefix-rule>
            """
        | Infix (s, t1, t2, pos) ->
            html $"""
                <infix-rule
                    infix={s}
                    position={pos}
                    rootName={rootName}
                    .transformation1={t1}
                    .transformation2={t2}>
                </infix-rule>
            """

[<LitElement("rule-element")>]
let RuleElement () =
    let host, props =
        LitElement.init (fun init ->
            init.props <- {|
                rootName = Prop.Of("")
                rule = Prop.Of(TRule emptyTransformation, attribute = "")
                ruleType = Prop.Of("transformation")
                ruleId = Prop.Of(0)
            |})

    let rule, setRule = Hook.useState props.rule.Value
    let ruleType = props.ruleType.Value

    let ruleRef = Hook.useRef<HTMLSelectElement>()

    html
        $"""
            <select {Lit.refValue ruleRef}
                @change={fun _ ->
                    let t =
                        match ruleRef.Value with
                        | None -> TRule emptyTransformation
                        | Some v ->
                            match v.value with
                            | "suffix" -> AffixRule (Suffix ("", emptyTransformation))
                            | "prefix" -> AffixRule (Prefix ("", emptyTransformation))
                            | "infix"  -> AffixRule (Infix ("", emptyTransformation, emptyTransformation, 0))
                            | _        -> TRule emptyTransformation
                    setRule t
                    host.dispatchCustomEvent ("rule-changed", t)
                    }>
                <option value="suffix" ?selected={ruleType = "suffix"}>Suffix</option>
                <option value="prefix" ?selected={ruleType = "prefix"}>Prefix</option>
                <option value="infix" ?selected={ruleType = "infix"}>Infix</option>
                <option value="transformation" ?selected={ruleType = "transformation"}>Transformation</option>
            </select>
            <div @rule-changed={fun (ev : CustomEvent) -> setRule (ev.detail :?> Rule)}
                style="display: inline-block;">
                {ruleTemplate rule props.rootName.Value}
            </div>
        """

let getRuleType r =
    match r with
    | TRule _ -> "transformation"
    | AffixRule a ->
        match a with
        | Suffix _ -> "suffix"
        | Prefix _ -> "prefix"
        | Infix _  -> "infix"

let ruleElement (i, rule) rootName =
    let ruleType = getRuleType rule
    html
        $"""
            <rule-element .rule={rule} ruleType={ruleType} ruleId={i} rootName={rootName}>
            <rule-element>
        """

let rec postAxisRules lid iid avid newRules =
    promise {
        do! server.postAxisRules iid avid newRules |> Async.StartAsPromise
        return! rulesTemplate lid
    }

and [<LitElement("axis-rules")>] AxisRules () =
    let _, props =
        LitElement.init (fun init ->
            init.styles <- [
                css $"""
                    div.axis-rules {{
                        border-bottom: 1px solid;
                        padding: 5px 5px 5px 5px;
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
    let sp = props.inflection.Value.speechPart
    let classes = props.inflection.Value.classes
    let iid = props.inflection.Value.id

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

    let ruleParentClass = "rule-parent"
    let rootName = "axis-rules"

    html
        $"""
        <div class=axis-rules>
            <div>
                {findAxisValueName <| fst rules}:
                {LitBindings.repeat (snd rules,
                                    (fun r -> $"{fst r}"),
                                    (fun r i -> html $"<div @rule-changed={(fun (ev : CustomEvent) ->
                                                            let a = ev.detail :?> Rule
                                                            replaceRule (fst r, a))}
                                                        class={ruleParentClass}
                                                        @rule-deleted={(fun _ -> deleteRule (fst r))}
                                                        draggable=true>
                                                            {i+1}: {ruleElement r rootName}
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
                    Lit.ofPromise(postAxisRules lid iid avid newRules, placeholder=loadingTemplate) |> Lit.render root
                )}
            >Submit
            </button>
        </div>
        """

and postOverrideRules lid iid newOverrideRules =
    promise {
        do! server.postOverrideRules iid newOverrideRules |> Async.StartAsPromise
        return! rulesTemplate lid
    }

and [<LitElement("override-rules")>] OverrideRules () =
    let _, props =
        LitElement.init (fun init ->
            init.styles <- [
                css $"""
                    table, td, th {{
                        border: 1px solid;
                        border-collapse: collapse;
                        padding: 5px 5px 5px 5px;
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
    let sp = props.inflection.Value.speechPart
    let classes = props.inflection.Value.classes |> Set.toSeq
    let iid = props.inflection.Value.id

    let inflectionAxesValues = props.inflection.Value.axes.axes |> List.map (fun a ->
        a.inflections |> Map.keys |> Seq.toList)

    let findAxisValueName avid =
        props.axesNames.Value |> List.tryFind (fun (k, _) -> k = avid)
                              |> Option.map snd |> Option.defaultValue ""

    let findAxesValues (axesNames : string) =
        let axesNameList = axesNames.Split(" ") |> Seq.toList
        axesNameList |> List.map (fun name ->
            props.axesNames.Value |> List.tryFind (fun (_, v) -> v = name)
                                  |> Option.map fst |> Option.defaultValue 0
        )

    let axesCombined = cartesianN inflectionAxesValues |> List.map (List.map findAxisValueName >> List.distinct)

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

    let onDragStart i (ev : DragEvent) =
        ev.dataTransfer.clearData() |> ignore
        ev.dataTransfer.setData("text/plain", i.ToString())

    let onDrop axes i (ev : DragEvent) =
        ev.preventDefault()
        let ruleI = ev.dataTransfer.getData("text") |> int
        let newRules = overrideRules |> List.map (fun (axes', rules') ->
            if axes' = axes then
                let rule = List.tryFind (fun (i, _) -> i = ruleI) rules'
                           |> Option.defaultValue (0, TRule emptyTransformation)
                let filteredRules = List.except [rule] rules'
                let ruleIndex = List.findIndex (fun (i', _) -> i' = i) filteredRules
                let newRules = List.insertAt ruleIndex rule filteredRules
                (axes', newRules)
            else (axes', rules')
        )
        setOverrideRules newRules

    let ruleParentClass = "rule-parent"
    let moveEffect = "move"
    let rootName = "override-rules"

    let rulesTemplate axes rules =
        html $"""
            <div>
            {LitBindings.repeat (rules,
                                (fun r -> $"{fst r}"),
                                (fun r i -> html $"<div @rule-changed={(fun (ev : CustomEvent) ->
                                                        let a = ev.detail :?> Rule
                                                        replaceOverrideRule axes (fst r, a))}
                                                        @rule-deleted={fun _ -> deleteRule axes (fst r)}
                                                        class={ruleParentClass}
                                                        draggable=true
                                                        @dragstart={fun (ev : DragEvent) ->
                                                            ev.stopPropagation()
                                                            ev.dataTransfer.effectAllowed <- moveEffect
                                                            onDragStart (fst r) ev
                                                        }
                                                        @dragover={fun (ev : DragEvent) -> ev.preventDefault()}
                                                        @drop={onDrop axes (fst r)}>
                                                        {i+1}: {ruleElement r rootName}
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

    let axesOption axesNames a =
        html $"""
            <option ?selected={Set axesNames = Set a}>
                {a |> String.concat " "}
            </option>"""

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
                                    html  $"<tr>
                                                <td>
                                                    <button
                                                        @click={fun _ -> deleteAxes axes}>
                                                        ❌
                                                    </button>
                                                    <select
                                                        @change={fun (ev : CustomEvent) ->
                                                            let newAxesNames = ev.target.Value
                                                            let newAxes = findAxesValues newAxesNames
                                                            replaceAxes axes newAxes
                                                        }>
                                                        {axesCombined |> List.map (fun a ->
                                                            axesOption axesNames a
                                                        )}
                                                    </select>
                                                </td>
                                                <td>{rulesTemplate axes rules}</td>
                                            </tr>"
                                )}
                <tr>
                    <td>
                        <button
                            @click={fun _ -> addAxes ()}>
                            Add axes
                        </button>
                        <button
                            @click={fun _ -> window.alert(overrideRules.ToString())}>
                            Get Rules
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
                    </td>
                </tr>
        </table>
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
                            <th>Axis</th>
                            <th>Inflections</th>
                        </tr>
                        <tr>
                            <td>{axis.name}</td>
                            <td>{axis.inflections |> Map.map (
                                    fun k v -> html $"<axis-rules
                                                        language={lid}
                                                        .inflection={inflection}
                                                        .rules={(k, List.mapi (fun i r -> (i, r)) v)}
                                                        .axesNames = {axes |> Seq.map (fun a -> a.values) |> Seq.concat |> Seq.toList}>
                                                      </axis-rules>"
                                ) |> Map.toList |> List.map snd}
                            </td>
                        </tr>
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
