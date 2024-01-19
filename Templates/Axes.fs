module OnlineConlangFront.Templates.Axes

open Browser
open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Loading

open Fable.Core

let private el = document.getElementById("root")

[<HookComponent>]
let rec addValueButton lid aid =
    let value, setValue = Hook.useState ""
    let inputRef = Hook.useRef<HTMLInputElement>()
    html
        $"""
            <input {Lit.refValue inputRef}
                value={value}
                @keyup={EvVal setValue}>
            <button
                @click={Ev(fun _ ->
                    match inputRef.Value with
                    | None -> ()
                    | Some ref -> match ref.value with
                                    | "" -> ()
                                    | v -> Lit.ofPromise(postAxisValue lid aid v, placeholder=loadingTemplate) |> Lit.render el
                )}>
                Add value
            </button>
        """

and deleteValueButton lid vid =
    html
        $"""
            <button
                @click={Ev(fun _ -> Lit.ofPromise(deleteAxisValue lid vid, placeholder=loadingTemplate) |> Lit.render el
                )}>
                ❌
            </button>
        """

and axesTemplate lid =
    promise {
        let! axes = server.getAxes lid |> Async.StartAsPromise
        return html
            $"""
                <table>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Values</th>
                        <th></th>
                    </tr>
                    {axes |> Seq.map (fun axis ->
                        let values = axis.values |> List.map (fun (aid, v) -> html $"{v} {deleteValueButton lid aid}<br>")
                        html  $"<tr>
                                    <td>{axis.id}</td>
                                    <td>{axis.name}</td>
                                    <td>{values}</td>
                                    <td>{addValueButton lid axis.id}</td>
                                </tr>"
                    )}
                </table>
            """
    }

and postAxisValue lid aid v =
    promise {
        let! _ = server.postAxisValue aid v |> Async.StartAsPromise
        return! axesTemplate lid
    }

and deleteAxisValue lid vid =
    promise {
        let! _ = server.deleteAxisValue vid |> Async.StartAsPromise
        return! axesTemplate lid
    }
