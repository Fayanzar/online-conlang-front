module OnlineConlangFront.Templates.Classes

open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Loading

open Fable.Core

type ClassForLit =
    {
        oldClassName : Class
        newClassName : Class
        classValues  : string seq
    }

[<LitElement("cl-value")>]
let rec ClassValues () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
            cl = Prop.Of("")
            clValue = Prop.Of("")
        |}
        init.styles <- OnlineConlangFront.Shared.styles
    )

    let cl = props.cl.Value
    let clValue, setClassValue = (props.clValue.Value, props.clValue.Value) |> Hook.useState
    let lid = props.language.Value

    html $"""
        <div>
            <input
                value={snd clValue}
                @keyup={fun (ev : CustomEvent) ->
                    setClassValue (fst clValue, ev.target.Value)
                }>
            <button
                @click={fun _ -> Lit.ofPromise(deleteClassValue lid cl (fst clValue), placeholder=loadingTemplate) |> Lit.render root}>
                ❌
            </button>
            <button
                @click={fun _ -> Lit.ofPromise(putClassValue lid cl (fst clValue) (snd clValue), placeholder=loadingTemplate) |> Lit.render root}>
                Submit
            </button>
        </div>
    """

and [<LitElement("add-cl-value")>] AddClassValue () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
            cl = Prop.Of("")
        |}
        init.styles <- OnlineConlangFront.Shared.styles
    )

    let lid = props.language.Value
    let cl = props.cl.Value
    let clValue, setClassValue = Hook.useState ""

    html $"""
        <div>
            <input
                value={clValue}
                @keyup={fun (ev : CustomEvent) -> setClassValue ev.target.Value}>
            <button
                @click={fun _ -> Lit.ofPromise(postClassValue lid cl clValue, placeholder=loadingTemplate) |> Lit.render root}>
                Add value
            </button>
        </div>
    """

and [<LitElement("add-cl")>] AddClass () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
        |}
        init.styles <- OnlineConlangFront.Shared.styles
    )

    let lid = props.language.Value
    let cl, setClass = Hook.useState ""

    html $"""
        <div>
            <input
                value={cl}
                @keyup={fun (ev : CustomEvent) -> setClass ev.target.Value}>
            <button
                @click={fun _ -> Lit.ofPromise(postClassName lid cl, placeholder=loadingTemplate) |> Lit.render root}>
                Add value
            </button>
        </div>
    """

and [<LitElement("classes-table")>] ClassesTable () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            language = Prop.Of(0)
            classes = Prop.Of(([] : (Class * string seq) list), attribute="")
        |}
        init.styles <- OnlineConlangFront.Shared.styles
    )

    let lid = props.language.Value
    let classes, setClasses = props.classes.Value
                            |> List.map (fun (className, classValues) ->
                                {
                                    oldClassName = className;
                                    newClassName = className;
                                    classValues = classValues
                                }) |> Hook.useState

    let setClass className newName =
        let newClasses = classes |> List.map (fun cl' ->
            if className = cl'.oldClassName then {
                                                    oldClassName = cl'.oldClassName;
                                                    newClassName = newName;
                                                    classValues = cl'.classValues
                                                }
                                            else cl'
        )
        setClasses newClasses

    let clValue av = html $"<cl-value
            language={lid}
            .clValue={av}>
        </cl-value>"

    html $"""
        <table>
            <tr>
                <th>Name</th>
                <th>Values</th>
            </tr>
            {classes |> Seq.map (fun cl ->
                html  $"<tr>
                            <td>
                                <input
                                    @keyup={fun (ev : CustomEvent) -> setClass cl.oldClassName ev.target.Value}
                                    value={cl.oldClassName}>
                                <button
                                    @click={fun _ -> Lit.ofPromise(putClassName lid cl.oldClassName cl.newClassName, placeholder=loadingTemplate)
                                                                    |> Lit.render root}>
                                    Submit
                                </button>
                            </td>
                            <td>
                                {cl.classValues |> Seq.map (fun cv ->
                                    clValue cv
                                )}
                                <add-cl-value
                                    language={lid}
                                    cl={cl.oldClassName}>
                                </add-cl-value>
                            </td>
                        </tr>"
            )}
        </table>
        <add-cl language={lid}></add-cl>
    """

and classesTemplate lid =
    promise {
        let! classes = server.getClasses lid |> Async.StartAsPromise
        return
            html $"""
                <classes-table
                    language={lid}
                    .classes={classes |> Map.toList}>
                </classes-table>
            """
    }

and postClassValue lid cl clv =
    promise {
        do! server.postClassValue getJWTCookie lid cl clv |> Async.StartAsPromise
        return! classesTemplate lid
    }

and deleteClassValue lid cl clv =
    promise {
        do! server.deleteClassValue getJWTCookie lid cl clv |> Async.StartAsPromise
        return! classesTemplate lid
    }

and putClassValue lid cl oldClv newClv =
    promise {
        do! server.putClassValue getJWTCookie lid cl oldClv newClv |> Async.StartAsPromise
        return! classesTemplate lid
    }

and putClassName lid oldName newName =
    promise {
        do! server.putClass getJWTCookie lid oldName newName |> Async.StartAsPromise
        return! classesTemplate lid
    }

and postClassName lid cl =
    promise {
        do! server.postClass getJWTCookie lid cl |> Async.StartAsPromise
        return! classesTemplate lid
    }
