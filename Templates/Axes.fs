module OnlineConlangFront.Templates.Axes

open Lit

open OnlineConlangFront.Foundation

open Fable.Core

let axesTemplate lid =
    promise {
        let! axes = server.getAxes lid |> Async.StartAsPromise
        return html $"<p>{axes}</p>"
    }
