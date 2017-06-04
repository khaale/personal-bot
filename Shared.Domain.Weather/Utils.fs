namespace PersonalBot.Shared.Domain.Weather.Utils

open FSharp.Data
open System

type CssSelector = 
    | CssSelector of string
    | CssSelectors of list<string>
type AttrName = AttrName of string

type HtmlSelector(html:HtmlDocument) = 

    let selectNode selector = 
        let innerSelect selectorString = 
            match (html.CssSelect selectorString) with 
            | x when not x.IsEmpty -> Some x.Head
            | _ -> None
        match selector with
        | CssSelector selectorString -> innerSelect selectorString            
        | CssSelectors selectors -> selectors |> List.tryPick innerSelect

    let selectNodeText = 
        selectNode >> Option.map (fun x -> (x.InnerText ()).Trim ())
    let selectNodeAttribute (AttrName attrName) = 
        selectNode >> Option.map (fun x -> (x.AttributeValue attrName).Trim ())

    member this.SelectNode = selectNode
    member this.SelectNodeText = selectNodeText
    member this.SelectNodeAttribute = selectNodeAttribute


