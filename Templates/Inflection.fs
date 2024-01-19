module OnlineConlangFront.Templates.Inflection

open System
open Fable.Core
open Browser
open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation

// interface
type Window =
    // function description
    abstract alert: ?message: string -> unit

// wiring-up JavaScript and F# with [<Global>] and jsNative
let [<Global>] window: Window = jsNative

let emptyTransformation = { input = ""; output = ""; applyMultiple = false }

[<HookComponent>]
let suffixRuleTemplate (s : string) t =
    let suffix, setSuffix = Hook.useState s
    let suffixRef = Hook.useRef<HTMLInputElement>()
    let input, setInput = Hook.useState t.input
    let inputRef = Hook.useRef<HTMLInputElement>()
    let output, setOutput = Hook.useState t.output
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
                        {if t.applyMultiple then "checked" else ""}>
                </td>
            </tr>
        </table>
        """

[<HookComponent>]
let prefixRuleTemplate (s : string) t =
    let prefix, setPrefix = Hook.useState s
    let prefixRef = Hook.useRef<HTMLInputElement>()
    let input, setInput = Hook.useState t.input
    let inputRef = Hook.useRef<HTMLInputElement>()
    let output, setOutput = Hook.useState t.output
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
                        {if t.applyMultiple then "checked" else ""}>
                </td>
            </tr>
        </table>
        """

[<HookComponent>]
let infixRuleTemplate (s : string) t1 t2 =
    let infix, setInfix = Hook.useState s
    let infixRef = Hook.useRef<HTMLInputElement>()
    let input1, setInput1 = Hook.useState t1.input
    let inputRef1 = Hook.useRef<HTMLInputElement>()
    let output1, setOutput1 = Hook.useState t1.output
    let outputRef1 = Hook.useRef<HTMLInputElement>()
    let input2, setInput2 = Hook.useState t2.input
    let inputRef2 = Hook.useRef<HTMLInputElement>()
    let output2, setOutput2 = Hook.useState t2.output
    let outputRef2 = Hook.useRef<HTMLInputElement>()
    let applyMultipleRef = Hook.useRef<HTMLInputElement>()
    html
        $"""
        <table id="rule">
            <tr>
                <th>Input</th>
                <th>Output</th>
                <th>Apply multiple</th>
                <th>Infix</th>
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
                    <input {Lit.refValue applyMultipleRef}
                        type="checkbox"
                        {if t1.applyMultiple then "checked" else ""}>
                </td>
                <td>
                    <input {Lit.refValue infixRef}
                        value={infix}
                        @keyup={EvVal setInfix}>
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
                    <input {Lit.refValue applyMultipleRef}
                        type="checkbox"
                        {if t2.applyMultiple then "checked" else ""}>
                </td>
            </tr>
        </table>
        """

[<HookComponent>]
let transformRuleTemplate t =
    let input, setInput = Hook.useState t.input
    let inputRef = Hook.useRef<HTMLInputElement>()
    let output, setOutput = Hook.useState t.output
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
                        {if t.applyMultiple then "checked" else ""}>
                </td>
            </tr>
        </table>
        """

let ruleTemplate rule =
    match rule with
    | TRule t -> transformRuleTemplate t
    | AffixRule a ->
        match a with
        | Suffix (s, t) -> suffixRuleTemplate s t
        | Prefix (s, t) -> prefixRuleTemplate s t
        | Infix (s, t1, t2, _) -> infixRuleTemplate s t1 t2


[<LitElement("rule-element")>]
let RuleElement () =
    let _, props =
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
                    }>
                <option value="suffix" ?selected={ruleType = "suffix"}>Suffix</option>
                <option value="prefix" ?selected={ruleType = "prefix"}>Prefix</option>
                <option value="infix" ?selected={ruleType = "infix"}>Infix</option>
                <option value="transformation" ?selected={ruleType = "transformation"}>Transformation</option>
            </select>
            <div>
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

[<LitElement("axis-rules")>]
let AxisRules () =
    let _, props =
        LitElement.init (fun init ->
            init.props <- {|
                rules = Prop.Of((0, ([] : (int * Rule) list)), attribute = "")
                axesNames = Prop.Of(([] : (int * string) list), attribute = "")
            |})

    let rules, setRules = Hook.useState props.rules.Value
    let findAxisValueName avid =
        props.axesNames.Value |> List.tryFind (fun (k, _) -> k = avid)
                              |> Option.map snd |> Option.defaultValue ""
    html
        $"""
            <div>
                {findAxisValueName <| fst rules}:
                <ul>
                    {LitBindings.repeat (snd rules,
                                        (fun r -> $"{fst r}"),
                                        (fun r i -> html $"<li>{i+1}: {ruleElement r}</li>"))
                    }
                </ul>
            </div>
            <button
                @click={Ev(fun _ ->
                    let nextInd = (snd rules |> List.map fst |> List.max) + 1
                    let newRule = (nextInd, TRule emptyTransformation)
                    setRules (fst rules, (snd rules) @ [newRule])
                )}
            >Add item</button>
            <button
                @click={Ev(fun _ -> window.alert(rules.ToString()))}
            >Submit
            </button>
        """

let inflectionTemplate lid  =
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
