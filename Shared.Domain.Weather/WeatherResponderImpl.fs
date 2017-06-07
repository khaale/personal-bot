module PersonalBot.Shared.Domain.Weather.WeatherResponderImpl

open System.Collections.Generic
open Microsoft.Bot.Connector

module private Messages =
    let createCard title text =
        let card = new HeroCard()
        card.Title <- title
        card.Text <- text
        card.Buttons <- new List<CardAction>()
        card

    let addUrlButton title value (card:HeroCard) = 
        let button = new CardAction()
        button.Title <- title
        button.Type <- "openUrl"
        button.Value <- value  
        card.Buttons.Add ( button )
        card

    let fillReply (reply:IMessageActivity) (cards:seq<Attachment>) =
        printfn "reply: %A" reply
        reply.AttachmentLayout <- AttachmentLayoutTypes.Carousel
        if reply.Attachments = null then reply.Attachments <- new List<Attachment>()
        cards |> Seq.iter (fun a -> reply.Attachments.Add a)
        reply
    
    let createRamblerCard (summary: RamblerWeather.TodayWeatherSummary) = 
        createCard (sprintf "Rambler: %s°C" summary.Temperature) (summary.Explanation) 
            |> addUrlButton "Details" summary.DetailsUri

    let createYandexCard (summary: YandexWeather.TodayWeatherSummary) = 
        createCard (sprintf "Yandex: %s" summary.Temperature) (sprintf "%s, %s." summary.ExplanationRain summary.Explanation)
            |> addUrlButton "Details" summary.DetailsUri
            |> addUrlButton "Rain Map" summary.RainMapUri
 
module Env = 
    type IEnvironment =                                              
        abstract member GetRamblerWeatherAsync : unit -> Async<RamblerWeather.TodayWeatherSummary>
        abstract member GetYandexWeatherAsync : unit -> Async<YandexWeather.TodayWeatherSummary>
        abstract member SendReplyAsync : IMessageActivity -> Async<unit>

    type private EAsync<'a> = IEnvironment -> Async<(IEnvironment*'a)>

    type EnvBuilderAsync() = 
        let abind fn v = async.Bind (v,fn)
        member x.Bind (computation:EAsync<'a>, binder:'a -> EAsync<'b>) (env:IEnvironment) : Async<IEnvironment*'b>= 
            let x' = fun (e, x) -> binder x e 
            (x' |> abind << computation) env
        member x.Return v e = async.Return (e,v)

    let private wrapEnv f (e:IEnvironment) = 
        async {  
            let! r = f e
            return (e,r)
        }

    let getRamblerWeatherAsync () = wrapEnv <| fun e -> e.GetRamblerWeatherAsync ()
    let getYandexWeatherAsync () = wrapEnv <| fun e -> e.GetYandexWeatherAsync ()
    let sendReplyAsync reply = wrapEnv <| fun e -> e.SendReplyAsync reply 

    let envAsync = EnvBuilderAsync()

let sendWeather reply = 
    Env.envAsync {
        let! rw = Env.getRamblerWeatherAsync ()
        let! yw = Env.getYandexWeatherAsync ()

        let rcard = Messages.createRamblerCard rw
        let ycard = Messages.createYandexCard yw
        
        do! [rcard; ycard] 
            |> Seq.map (fun x -> x.ToAttachment()) 
            |> Messages.fillReply reply
            |> Env.sendReplyAsync

        return true
    }