namespace PersonalBot.Shared.Domain.Weather

open Microsoft.Bot.Connector
open PersonalBot.Shared.Core.Responders

type WeatherResponder(sender: IMessageSender) =
    
    let createCard (summary: RamblerWeather.TodayWeatherSummary) = 
        let card = new HeroCard ()
        card.Title <- summary.Explanation
        card.Text <- summary.Temperature
        card

    member this.ProcessAsync (reply: IMessageActivity) = 
        async {
            let! summary = RamblerWeather.getTodaySummaryAsync

            let card = createCard summary
            
            if reply.Attachments = null 
            then reply.Attachments <- (new System.Collections.Generic.List<Attachment> () :> System.Collections.Generic.IList<Attachment>)

            reply.Attachments.Add (card.ToAttachment ())

            return true
        } |> Async.StartAsTask
