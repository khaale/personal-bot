namespace PersonalBot.Shared.Domain.Weather.Utils

open FSharp.Data
open System

type CssSelector = CssSelector of string
type AttrName = AttrName of string

type HtmlSelector(html:HtmlDocument) = 

    let selectNode (CssSelector selector) =
            match (html.CssSelect selector) with 
            | x when not x.IsEmpty -> Some x.Head
            | _ -> None
    let selectNodeText = 
        selectNode >> Option.map (fun x -> (x.InnerText ()).Trim ())
    let selectNodeAttribute (AttrName attrName) = 
        selectNode >> Option.map(fun x -> (x.AttributeValue attrName).Trim ())

    member this.SelectNode = selectNode
    member this.SelectNodeText = selectNodeText
    member this.SelectNodeAttribute = selectNodeAttribute


