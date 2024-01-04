module OnlineConlangFront.Templates.Loading

open Lit

[<HookComponent>]
let loadingTemplate =
    html $"""
        <div class="loader"></div>
    """
