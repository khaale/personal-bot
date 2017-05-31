namespace PersonalBot.Shared.Domain.Weather

open FSharp.Data
open Microsoft.Bot.Connector
open PersonalBot.Shared.Core.Responders
open PersonalBot.Shared.Domain.Weather.Utils

type RamblerWeather = HtmlProvider<"./data/weather.rambler.ru.html">
type TodayWeatherSummary = { Explanation: string; Temperature: string; DetailsUri: string }

type WeatherResponder(sender:IMessageSender) =

    let ramblerWeatherUri = "https://weather.rambler.ru/v-samare/"

    let getTodaySummaryAsync = 
        async {
            let! page = RamblerWeather.AsyncLoad ramblerWeatherUri

            let selector = HtmlSelector page.Html
            
            let temp = selector.SelectNodeText (CssSelector "span.weather-now__value")
            let desc = selector.SelectNodeText (CssSelector ".weather-today__explanation")
            let detailsUri = selector.SelectNodeAttribute (AttrName "href") (CssSelector ".weather-today__meta a")

            let getDefaulValue = 
                Option.defaultValue "Unknown"

            return { 
                Explanation = getDefaulValue desc; 
                Temperature = getDefaulValue temp; 
                DetailsUri = getDefaulValue detailsUri 
            }
        }
    
    let createCard (summary:TodayWeatherSummary) = 
        let card = new HeroCard ()
        card.Title <- summary.Explanation
        card.Text <- summary.Temperature
        card

    member this.ProcessAsync (reply:IMessageActivity) = 
        async {
            let! summary = getTodaySummaryAsync

            let card = createCard summary
            
            if reply.Attachments = null 
            then reply.Attachments <- (new System.Collections.Generic.List<Attachment> () :> System.Collections.Generic.IList<Attachment>)

            reply.Attachments.Add (card.ToAttachment ())

            return true
        } |> Async.StartAsTask
