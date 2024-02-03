module OnlineConlangFront.Templates.InflectionRules

open Browser
open Browser.Types
open Lit

open SharedModels

let emptyTransformation = { input = ""; output = ""; applyMultiple = false }

let emptyInflection = { language = 0
                      ; speechPart = ""
                      ; classes = Set.empty
                      ; axes = { axes = []; overrides = [] }
                      ; id = 0
                      ; name = None
                      }

[<LitElement("rule-delete")>]
let RuleDelete () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- [
                yield! OnlineConlangFront.Shared.styles

                css $"""
                    button.rule-delete-infix {{
                        vertical-align: top;
                        margin-top: 1.33rem;
                    }}
                """
            ]
            init.props <- {|
                isInfix = Prop.Of("")
            |}
        )

    let isInfix = props.isInfix.Value
    let infixClass = if isInfix = "true" then "rule-delete-infix" else ""

    html $"""
        <button class="rule-delete {infixClass}"
            @click={fun _ -> host.dispatchEvent ("rule-deleted")}>
            ❌
        </button>
    """

[<LitElement("suffix-rule")>]
let SuffixRule () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- [
                yield! OnlineConlangFront.Shared.styles

                css $"""
                    input:not([type='checkbox']) {{
                        width: 70px;
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
            <input {Lit.refValue suffixRef} class=suffix
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
            <input {Lit.refValue inputRef} class=t-input
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
            <input {Lit.refValue outputRef} class=t-output
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
            <label>wᵢ→…→wᵢ→…</label>
            <div class=apply-multiple-checkbox>
                <input {Lit.refValue applyMultipleRef} class="tgl tgl-light" id="amc"
                    type="checkbox"
                    ?checked={props.transformation.Value.applyMultiple}>
                <label class="tgl-btn" for=amc></label>
            </div>
        """

[<LitElement("prefix-rule")>]
let PrefixRule () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- [
                yield! OnlineConlangFront.Shared.styles

                css $"""
                    input:not([type='checkbox']) {{
                        width: 70px;
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
            <input {Lit.refValue prefixRef} class=prefix
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
            <input {Lit.refValue inputRef} class=t-input
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
            <input {Lit.refValue outputRef} class=t-output
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

            <label>wᵢ→…→wᵢ→…</label>
            <div class=apply-multiple-checkbox>
                <input {Lit.refValue applyMultipleRef} class="tgl tgl-light" id="amc"
                    type="checkbox"
                    ?checked={props.transformation.Value.applyMultiple}>
                <label class="tgl-btn" for=amc></label>
        """

[<LitElement("infix-rule")>]
let InfixRule () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- [
                yield! OnlineConlangFront.Shared.styles

                css $"""
                    input:not([type='checkbox']) {{
                        width: 70px;
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
            <input {Lit.refValue inputRef1} class=t-input
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
            <input {Lit.refValue outputRef1} class=t-output
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

            <label>wᵢ→…→wᵢ→…</label>
            <div class=apply-multiple-checkbox>
                <input {Lit.refValue applyMultipleRef1} class="tgl tgl-light" id="amc1"
                    type="checkbox"
                    ?checked={props.transformation1.Value.applyMultiple}>
                <label class="tgl-btn" for=amc1></label>
            </div>
            <br>
            <input {Lit.refValue inputRef2} class=t-input
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
            <input {Lit.refValue outputRef2} class=t-output
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

            <label>wᵢ→…→wᵢ→…</label>
            <div class=apply-multiple-checkbox>
                <input {Lit.refValue applyMultipleRef2} class="tgl tgl-light" id="amc2"
                    type="checkbox"
                    ?checked={props.transformation2.Value.applyMultiple}>
                <label class="tgl-btn" for=amc2></label>
            </div>

            ; w<
            <input {Lit.refValue infixRef} class=infix
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
            <input {Lit.refValue positionRef} type=number class=infix-pos
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
            >ord
        """

[<LitElement("transform-rule")>]
let TransformRule () =
    let host, props =
        LitElement.init (fun init ->
            init.styles <- [
                yield! OnlineConlangFront.Shared.styles

                css $"""
                    input:not([type='checkbox']) {{
                        width: 70px;
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
            <input class=t-input {Lit.refValue inputRef}
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
            <input class=t-output {Lit.refValue outputRef}
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

            <label>wᵢ→…→wᵢ→…</label>
            <div class=apply-multiple-checkbox>
                <input {Lit.refValue applyMultipleRef} class="tgl tgl-light" id="amc"
                    type="checkbox"
                    ?checked={props.transformation.Value.applyMultiple}>
                <label class="tgl-btn" for=amc></label>
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
            init.styles <- [
                yield! OnlineConlangFront.Shared.styles

                css $"""
                    div.infix-select {{
                        vertical-align: top;
                        margin-top: 1.33rem;
                    }}
                """
            ]
            init.props <- {|
                rootName = Prop.Of("")
                rule = Prop.Of(TRule emptyTransformation, attribute = "")
                ruleType = Prop.Of("transformation")
                ruleId = Prop.Of(0)
            |})

    let rule, setRule = Hook.useState props.rule.Value
    let ruleType = props.ruleType.Value

    let ruleRef = Hook.useRef<HTMLSelectElement>()

    let isInfix =
        match rule with
            | AffixRule (Infix _) -> true
            | _ -> false

    let infixClass = if isInfix then "infix-select" else ""
    let infixStyle = if isInfix then "line-height: 2.33rem;" else ""

    html
        $"""
            <rule-delete isInfix={isInfix}></rule-delete>
            <div class="select {infixClass}">
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
                        host.dispatchCustomEvent ("rule-type-changed", t)
                        }>
                    <option value="suffix" ?selected={ruleType = "suffix"}>Suffix</option>
                    <option value="prefix" ?selected={ruleType = "prefix"}>Prefix</option>
                    <option value="infix" ?selected={ruleType = "infix"}>Infix</option>
                    <option value="transformation" ?selected={ruleType = "transformation"}>Transformation</option>
                </select>
            </div>
            <div @rule-changed={fun (ev : CustomEvent) -> setRule (ev.detail :?> Rule)}
                style="display: inline-block; {infixStyle}">
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
            <rule-element style="width: 100%%;"
                .rule={rule}
                ruleType={ruleType}
                ruleId={i}
                rootName={rootName}>
            <rule-element>
        """
