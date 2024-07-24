module OnlineConlangFront.Templates.Languages

open Browser.Types
open Lit

open SharedModels

open OnlineConlangFront.Foundation
open OnlineConlangFront.Templates.Loading

open Fable.Core

[<LitElement("language-table")>]
let rec LanguageTable () =
    let _, props = LitElement.init (fun init ->
        init.props <- {|
            languages = Prop.Of(([] : Language list), attribute="")
        |}
        init.styles <- OnlineConlangFront.Shared.styles
        )

    let languages, setLanguages = Hook.useState props.languages.Value
    let newLang, setNewLang = Hook.useState ""
    let newLangRef = Hook.useRef<HTMLInputElement>()

    html $"""
        <table>
            <tr>
                <th colspan="2">Languages</th>
            </tr>
        {languages |> List.map (fun lang ->
        html $"
            <tr>
                <td>{lang.id}</td>
                <td>{lang.name}</td>
            </tr>
        "
        )}
        </table>
        <div>
            <input {Lit.refValue newLangRef}
                value={newLang}
                @keyup={fun (ev : Event) ->
                    setNewLang ev.target.Value
            }>
        </div>
        <div>
            <button
                @click={fun _ ->
                    Lit.ofPromise(postLanguage newLang, placeholder=loadingTemplate) |> Lit.render root
                }
            >Create language
            </button>
        </div>
    """

and languagesTemplate () =
    promise {
        let! languages = server.getLanguages |> Async.StartAsPromise
        return html $"
            <language-table
                .languages={languages}
            ></language-table>
        "
    }

and postLanguage lang =
    promise {
        let! stoken = server.postLanguage getJWTCookie lang |> Async.StartAsPromise
        setJWTCookie stoken
        return! languagesTemplate ()
    }