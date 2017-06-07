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

    let addUrlButton (card:HeroCard) title value = 
        let button = new CardAction()
        button.Title <- title
        button.Type <- "openUrl"
        button.Value <- value  
        card.Buttons.Add ( button )

    let fillReply (reply:IMessageActivity) (cards:seq<Attachment>) =
        printfn "reply: %A" reply
        reply.AttachmentLayout <- AttachmentLayoutTypes.Carousel
        if reply.Attachments = null then reply.Attachments <- new List<Attachment>()
        cards |> Seq.iter (fun a -> reply.Attachments.Add a)
    
    let createRamblerCard (summary: RamblerWeather.TodayWeatherSummary) = 
        let card = createCard (sprintf "Rambler: %s°C" summary.Temperature) (summary.Explanation)
        addUrlButton card "Details" summary.DetailsUri
        card

    let createYandexCard (summary: YandexWeather.TodayWeatherSummary) = 
        let card = createCard (sprintf "Yandex: %s" summary.Temperature) (sprintf "%s, %s." summary.ExplanationRain summary.Explanation)
        addUrlButton card "Details" summary.DetailsUri
        addUrlButton card "Rain Map" summary.RainMapUri
        card
 
module Env = 
    type IEnvironment =                                              
        abstract member GetRamblerWeather : unit -> Async<RamblerWeather.TodayWeatherSummary>
        abstract member GetYandexWeather : unit -> Async<YandexWeather.TodayWeatherSummary>
        abstract member SendReply : IMessageActivity -> Async<unit>

    type EAsync<'a> = IEnvironment -> Async<(IEnvironment*'a)>

    type EnvBuilderAsync() = 
        let abind fn v = async.Bind (v,fn)
        member x.Bind (computation:EAsync<'a>, binder:'a -> EAsync<'b>) (env:IEnvironment) : Async<IEnvironment*'b>= 
            let x' = fun (e, x) -> binder x e 
            (x' |> abind << computation) env
        member x.Return v e = async.Return (e,v)

    let wrapAsync f e = 
        async {  
            let! r = f e
            return (e,r)
        }

    let getRamblerWeatherAsync () (env:IEnvironment) = wrapAsync (fun (e:IEnvironment) -> e.GetRamblerWeather ()) env
    let getYandexWeatherAsync () (env:IEnvironment) = wrapAsync (fun (e:IEnvironment) -> e.GetYandexWeather ()) env
    let sendReplyAsync reply (env:IEnvironment) = wrapAsync (fun (e:IEnvironment) -> e.SendReply reply) env 

    let envAsync = EnvBuilderAsync()

let sendWeather reply = 
    Env.envAsync {
        let! rw = Env.getRamblerWeatherAsync ()
        let! yw = Env.getYandexWeatherAsync ()

        let rcard = Messages.createRamblerCard rw
        let ycard = Messages.createYandexCard yw
        [rcard; ycard] 
            |> Seq.map (fun x -> x.ToAttachment()) 
            |> Messages.fillReply reply

        do! Env.sendReplyAsync reply

        return true
    }