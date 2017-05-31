// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "../packages/FSharp.Data.2.3.3/lib/net40/FSharp.Data.dll"
open FSharp.Data

let ramblerWeatherUri = "https://weather.rambler.ru/v-samare/"

type RamblerWeather = HtmlProvider<"./data/weather.rambler.ru.html">

type CssSelector = CssSelector of string
type AttrName = AttrName of string

let site = RamblerWeather.Load ramblerWeatherUri
(*
let selectNode (CssSelector selector) =
     match (site.Html.CssSelect selector) with 
        | x when not x.IsEmpty -> Some x.Head
        | _ -> None

let selectNodeText = selectNode >> Option.map (fun x -> (x.InnerText ()).Trim ())

let selectNodeAttribute (AttrName attrName) = selectNode >> Option.map(fun x -> (x.AttributeValue attrName).Trim ())

let temp = selectNodeText (CssSelector "span.weather-now__value")
let desc = selectNodeText (CssSelector ".weather-today__explanation")
let detailsUri = selectNodeAttribute (AttrName "href") (CssSelector ".weather-today__meta a")


let getRamblerWeatherPage =
        RamblerWeather.AsyncLoad ramblerWeatherUri

let ramblerWeatherPage = getRamblerWeatherPage |> Async.RunSynchronously
*)

type PageBuilder(html:HtmlDocument) =
    member this.Return x = x
    member this.Bind(x,f) = f x 


let selectNode (CssSelector selector) (html:HtmlDocument) = 
        match (html.CssSelect selector) with 
        | x when not x.IsEmpty -> Some x.Head
        | _ -> None

let selectNodeText selector html = 
    selectNode selector html |> Option.map (fun x -> (x.InnerText ()).Trim ())

   (*
let selectNodeText = 
    selectNode >> Option.map (fun x -> (x.InnerText ()).Trim ())
let selectNodeAttribute html (AttrName attrName) = 
    selectNode >> Option.map(fun x -> (x.AttributeValue attrName).Trim ())
    *)
let pageCtx = PageBuilder (site.Html)
let result = pageCtx {
    let! x = selectNode (CssSelector "span.weather-now__value")
    return x
}

result site.Html


let x = null
not (isNull x) 
x <> null



let notNull x = x |> isNull |> not
notNull x

let nn = isNull |> not
not <| isNull x