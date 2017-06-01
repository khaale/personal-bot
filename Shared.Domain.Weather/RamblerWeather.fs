module PersonalBot.Shared.Domain.Weather.RamblerWeather

open FSharp.Data
open PersonalBot.Shared.Domain.Weather.Utils

type RamblerWeatherProvider = HtmlProvider<"./data/weather.rambler.ru.html">
type TodayWeatherSummary = { Explanation: string; Temperature: string; DetailsUri: string }

let ramblerWeatherUri = "https://weather.rambler.ru/v-samare/"

let getTodaySummaryAsync = 
    async {
        let! page = RamblerWeatherProvider.AsyncLoad ramblerWeatherUri

        let selector = HtmlSelector page.Html
            
        let temp = selector.SelectNodeText (CssSelector "span.weather-now__value")
        let desc = selector.SelectNodeText (CssSelector ".weather-today__explanation")
        let detailsUri = selector.SelectNodeAttribute (AttrName "href") (CssSelector ".weather-today__meta a")

        let getDefaulValue = function
            | Some x -> x
            | None -> "Unknown"

        return { 
            Explanation = getDefaulValue desc; 
            Temperature = getDefaulValue temp; 
            DetailsUri = getDefaulValue detailsUri 
        }
    }