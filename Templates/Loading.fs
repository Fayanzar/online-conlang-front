module OnlineConlangFront.Templates.Loading

open Lit

[<LitElement("loading-element")>]
let LoadingElement () =
    let _ = LitElement.init (fun init ->
        init.styles <- [
            css $"""
                .loader {{
                    border: 16px solid #f3f3f3;
                    border-top: 16px solid #3498db;
                    width: 120px;
                    height: 120px;
                    animation: spin 2s linear infinite;
                    position: absolute;
                    border-radius: 50%%;
                    top: 50%%;
                    left: 50%%;
                }}

                @keyframes spin {{
                    from {{
                        transform: translate(-50%%, -50%%) rotate(0deg);
                    }}
                    to {{
                        transform: translate(-50%%, -50%%) rotate(360deg);
                    }}
                }}
            """
        ]
    )
    html $"<div class=loader></div>"

let loadingTemplate =
    html $"""
        <loading-element></loading-element>
    """
