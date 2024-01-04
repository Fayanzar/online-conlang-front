module OnlineConlangFront.Foundation

open SharedModels

open Fable.Remoting.Client

let server : IServer =
    Remoting.createApi()
    |> Remoting.withBaseUrl "http://localhost:5000"
    |> Remoting.withRouteBuilder routeBuilder
    |> Remoting.buildProxy<IServer>
