namespace PersonalBot.Shared.Domain.Weather

open System.Collections.Generic
open Microsoft.Bot.Connector
open PersonalBot.Shared.Core.Responders

type WeatherResponder(sender: IMessageSender) =
    
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

    let createRamblerCard (summary: RamblerWeather.TodayWeatherSummary) = 
        let card = createCard (sprintf "Rambler: %s°C" summary.Temperature) (summary.Explanation)
        addUrlButton card "Details" summary.DetailsUri
        card.ToAttachment()

    let createYandexCard (summary: YandexWeather.TodayWeatherSummary) = 
        let card = createCard (sprintf "Yandex: %s" summary.Temperature) (sprintf "%s, %s." summary.ExplanationRain summary.Explanation)
        addUrlButton card "Details" summary.DetailsUri
        addUrlButton card "Rain Map" summary.RainMapUri
        card.ToAttachment()

    let fillReply (reply:IMessageActivity) (cards:seq<Attachment>) = 
        reply.AttachmentLayout <- AttachmentLayoutTypes.Carousel
        if reply.Attachments = null
        then reply.Attachments <- new List<Attachment>()
        cards |> Seq.iter (fun a -> reply.Attachments.Add a)

    member this.ProcessAsync (reply: IMessageActivity) = 
        async {
            let! rdataFn = Async.StartChild RamblerWeather.getTodaySummaryAsync
            let! ydataFn = Async.StartChild YandexWeather.getTodaySummaryAsync            
            let! rdata = rdataFn
            let! ydata = ydataFn

            let rcard = createRamblerCard rdata
            let ycard = createYandexCard ydata
            fillReply reply [rcard; ycard]

            do! sender.SendAsync reply |> Async.AwaitTask

            return true
        } |> Async.StartAsTask
