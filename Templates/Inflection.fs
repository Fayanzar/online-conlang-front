module OnlineConlangFront.Templates.Inflection

open Fable.Core
open Browser
open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation

open OnlineConlangFront.Templates.Loading

// interface
type Window =
    // function description
    abstract alert: ?message: string -> unit

// wiring-up JavaScript and F# with [<Global>] and jsNative
let [<Global>] window: Window = jsNative

let el = document.getElementById("root")

let emptyTransformation = { input = ""; output = ""; applyMultiple = false }

[<LitElement("suffix-rule")>]
let SuffixRule () =
    let host, props =
        LitElement.init (fun init ->
            init.props <- {|
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
    html
        $"""
        <table id="rule">
            <tr>
                <th>Suffix</th>
                <th>Input</th>
                <th>Output</th>
                <th>Apply multiple</th>
            </tr>
            <tr>
                <td>
                    <input {Lit.refValue suffixRef}
                        value={suffix}
                        @keyup={EvVal setSuffix}>
                </td>
                <td>
                    <input {Lit.refValue inputRef}
                        value={input}
                        @keyup={EvVal setInput}>
                </td>
                <td>
                    <input {Lit.refValue outputRef}
                        value={output}
                        @keyup={EvVal setOutput}>
                </td>
                <td>
                    <input {Lit.refValue applyMultipleRef}
                        type="checkbox"
                        ?checked={props.transformation.Value.applyMultiple}>
                </td>
                <td>
                    <button
                        @click={fun _ ->
                            let newInput = inputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
                            let newOutput = outputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
                            let newApplyMultiple = applyMultipleRef.Value |> Option.map (fun v -> v.checked) |> Option.defaultValue false
                            let newSuffix = suffixRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
                            let newT = { input = newInput; output = newOutput; applyMultiple = newApplyMultiple }
                            let newR = AffixRule (Suffix (newSuffix, newT))
                            host.dispatchCustomEvent ("rule-changed", newR)
                        }>
                        Save
                    </button>
                </td>
            </tr>
        </table>
        """

[<LitElement("prefix-rule")>]
let PrefixRule () =
    let host, props =
        LitElement.init (fun init ->
            init.props <- {|
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
    html
        $"""
        <table id="rule">
            <tr>
                <th>Prefix</th>
                <th>Input</th>
                <th>Output</th>
                <th>Apply multiple</th>
            </tr>
            <tr>
                <td>
                    <input {Lit.refValue prefixRef}
                        value={prefix}
                        @keyup={EvVal setPrefix}>
                </td>
                <td>
                    <input {Lit.refValue inputRef}
                        value={input}
                        @keyup={EvVal setInput}>
                </td>
                <td>
                    <input {Lit.refValue outputRef}
                        value={output}
                        @keyup={EvVal setOutput}>
                </td>
                <td>
                    <input {Lit.refValue applyMultipleRef}
                        type="checkbox"
                        ?checked={props.transformation.Value.applyMultiple}>
                </td>
                <td>
                    <button
                        @click={fun _ ->
                            let newInput = inputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
                            let newOutput = outputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
                            let newApplyMultiple = applyMultipleRef.Value |> Option.map (fun v -> v.checked) |> Option.defaultValue false
                            let newPrefix = prefixRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
                            let newT = { input = newInput; output = newOutput; applyMultiple = newApplyMultiple }
                            let newR = AffixRule (Prefix (newPrefix, newT))
                            host.dispatchCustomEvent ("rule-changed", newR)
                        }>
                        Save
                    </button>
                </td>
            </tr>
        </table>
        """

[<LitElement("infix-rule")>]
let InfixRule () =
    let host, props =
        LitElement.init (fun init ->
            init.props <- {|
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
    html
        $"""
        <table id="rule">
            <tr>
                <th>Input</th>
                <th>Output</th>
                <th>Apply multiple</th>
                <th>Infix</th>
                <th>Position</th>
                <th>Input</th>
                <th>Output</th>
                <th>Apply multiple</th>
            </tr>
            <tr>
                <td>
                    <input {Lit.refValue inputRef1}
                        value={input1}
                        @keyup={EvVal setInput1}>
                </td>
                <td>
                    <input {Lit.refValue outputRef1}
                        value={output1}
                        @keyup={EvVal setOutput1}>
                </td>
                <td>
                    <input {Lit.refValue applyMultipleRef1}
                        type="checkbox"
                        ?checked={props.transformation1.Value.applyMultiple}>
                </td>
                <td>
                    <input {Lit.refValue infixRef}
                        value={infix}
                        @keyup={EvVal setInfix}>
                </td>
                <td>
                    <input {Lit.refValue positionRef} type=number
                        value={position}
                        @keyup={EvVal (setPosition << int)}>
                </td>
                <td>
                    <input {Lit.refValue inputRef2}
                        value={input2}
                        @keyup={EvVal setInput2}>
                </td>
                <td>
                    <input {Lit.refValue outputRef2}
                        value={output2}
                        @keyup={EvVal setOutput2}>
                </td>
                <td>
                    <input {Lit.refValue applyMultipleRef2}
                        type="checkbox"
                        ?checked={props.transformation2.Value.applyMultiple}>
                </td>
                <td>
                    <button
                        @click={fun _ ->
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
                        }>
                        Save
                    </button>
                </td>
            </tr>
        </table>
        """

[<LitElement("transform-rule")>]
let TransformRule () =
    let host, props =
        LitElement.init (fun init ->
            init.props <- {|
                transformation = Prop.Of(emptyTransformation, attribute = "")
            |})

    let input, setInput = Hook.useState props.transformation.Value.input
    let inputRef = Hook.useRef<HTMLInputElement>()
    let output, setOutput = Hook.useState props.transformation.Value.output
    let outputRef = Hook.useRef<HTMLInputElement>()
    let applyMultipleRef = Hook.useRef<HTMLInputElement>()
    html
        $"""
        <table id="rule">
            <tr>
                <th>Input</th>
                <th>Output</th>
                <th>Apply multiple</th>
            </tr>
            <tr>
                <td>
                    <input {Lit.refValue inputRef}
                        value={input}
                        @keyup={EvVal setInput}>
                </td>
                <td>
                    <input {Lit.refValue outputRef}
                        value={output}
                        @keyup={EvVal setOutput}>
                </td>
                <td>
                    <input {Lit.refValue applyMultipleRef}
                        type="checkbox"
                        ?checked={props.transformation.Value.applyMultiple}>
                </td>
                <td>
                    <button
                        @click={fun _ ->
                            let newInput = inputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
                            let newOutput = outputRef.Value |> Option.map (fun v -> v.value) |> Option.defaultValue ""
                            let newApplyMultiple = applyMultipleRef.Value |> Option.map (fun v -> v.checked) |> Option.defaultValue false
                            let newT = { input = newInput; output = newOutput; applyMultiple = newApplyMultiple}
                            host.dispatchCustomEvent ("rule-changed", TRule newT)
                        }>
                        Save
                    </button>
                </td>
            </tr>
        </table>
        """

let ruleTemplate rule =
    match rule with
    | TRule t ->
        html $"<transform-rule .transformation={t}></transform-rule>"
    | AffixRule a ->
        match a with
        | Suffix (s, t) ->
            html $"""
                <suffix-rule suffix={s} .transformation={t}></suffix-rule>
            """
        | Prefix (s, t) ->
            html $"""
                <prefix-rule prefix={s} .transformation={t}></prefix-rule>
            """
        | Infix (s, t1, t2, pos) ->
            html $"""
                <infix-rule
                    infix={s}
                    position={pos}
                    .transformation1={t1}
                    .transformation2={t2}>
                </infix-rule>
            """


[<LitElement("rule-element")>]
let RuleElement () =
    let host, props =
        LitElement.init (fun init ->
            init.props <- {|
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
            <div @rule-changed={fun (ev : CustomEvent) -> setRule (ev.detail :?> Rule)}>
                {ruleTemplate rule}
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

let ruleElement (i, rule) =
    let ruleType = getRuleType rule
    html
        $"""
            <rule-element .rule={rule} ruleType={ruleType} ruleId={i}>
            <rule-element>
        """

let rec postAxisRules lid avid newRules =
    promise {
        let! rules = server.getAxisRules avid |> Async.StartAsPromise
        let oldRulesLength = Map.count rules
        let newRulesLength = List.length newRules
        if newRulesLength >= oldRulesLength then
            let (part1, part2) = List.splitAt oldRulesLength newRules
            let rulesCombined = List.zip (rules |> Map.toList) part1
            for ((i, rule), newRule) in rulesCombined do
                if rule <> newRule then do! server.putAxisRule i newRule |> Async.StartAsPromise
            for rule in part2 do
                do! server.postAxisRule avid rule |> Async.StartAsPromise
        else
            let (part1, part2) = rules |> Map.toList |> List.splitAt newRulesLength
            let rulesCombined = List.zip part1 newRules
            for ((i, rule), newRule) in rulesCombined do
                if rule <> newRule then do! server.putAxisRule i newRule |> Async.StartAsPromise
            for (i, _) in part2 do
                do! server.deleteAxisRule i |> Async.StartAsPromise
        return! inflectionTemplate lid
    }

and [<LitElement("axis-rules")>] AxisRules () =
    let _, props =
        LitElement.init (fun init ->
            init.props <- {|
                language = Prop.Of(0)
                rules = Prop.Of((0, ([] : (int * Rule) list)), attribute = "")
                axesNames = Prop.Of(([] : (int * string) list), attribute = "")
            |})

    let rules, setRules = Hook.useState props.rules.Value
    let findAxisValueName avid =
        props.axesNames.Value |> List.tryFind (fun (k, _) -> k = avid)
                              |> Option.map snd |> Option.defaultValue ""

    let replaceRule (i, r) =
        let newRules = snd rules |> List.map (fun (i', r') -> if i = i' then (i, r) else (i', r'))
        setRules (fst rules, newRules)

    html
        $"""
            <div>
                {findAxisValueName <| fst rules}:
                <ul>
                    {LitBindings.repeat (snd rules,
                                        (fun r -> $"{fst r}"),
                                        (fun r i -> html $"<li @rule-changed={(fun (ev : CustomEvent) ->
                                                            let a = ev.detail :?> Rule
                                                            replaceRule (fst r, a))}>
                                                                {i+1}: {ruleElement r}
                                                           </li>"))
                    }
                </ul>
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
                    let lid = props.language.Value
                    let avid = fst rules
                    let newRules = snd rules |> List.map snd
                    Lit.ofPromise(postAxisRules lid avid newRules, placeholder=loadingTemplate) |> Lit.render el
                )}
            >Submit
            </button>
        """

and inflectionTemplate lid  =
    promise {
        let! inflections = server.getInflections lid |> Async.StartAsPromise
        let! axes = server.getAxes lid |> Async.StartAsPromise
        let findAxisValueName vid =
            let allValues = axes |> Seq.map (fun a -> a.values) |> Seq.concat
            allValues |> Seq.tryFind (fun (k, _) -> k = vid) |> Option.map snd |> Option.defaultValue ""
        let inflection = inflections |> Seq.find (fun i -> i.language = lid)
        let axesTable (axis : Axis) =
            html
                $"""
                    <table>
                        <tr>
                            <th>Name</th>
                            <th>Inflections</th>
                        </tr>
                        <tr>
                            <td>{axis.name}</td>
                            <td>{axis.inflections |> Map.map (
                                    fun k v -> html $"<axis-rules
                                                        language={lid}
                                                        .rules={(k, List.mapi (fun i r -> (i, r)) v)}
                                                        .axesNames = {axes |> Seq.map (fun a -> a.values) |> Seq.concat |> Seq.toList}>
                                                      </axis-rules>"
                                ) |> Map.toList |> List.map snd}
                            </td>
                        </tr>
                    </table>
                """
        return html
            $"""
                <table>
                    <tr>
                        <th>Part of speech</th>
                        <th>Classes</th>
                        <th>Axes</th>
                        <th>Overrides</th>
                    </tr>
                    <tr>
                        <td>{inflection.speechPart}</td>
                        <td>{inflection.classes}</td>
                        <td>{inflection.axes.axes |> List.map (fun a -> axesTable a)}</td>
                        <td>{inflection.axes.overrides |> Map.map (
                            fun k v ->
                                let overrideAxes = k |> List.map findAxisValueName |> String.concat " "
                                html $"{overrideAxes} : {v}<br>"
                            ) |> Map.toList |> List.map snd}
                        </td>
                    </tr>
                </table>
            """
    }
