module PersonalBot.Shared.Domain.Weather.YandexWeather

open FSharp.Data
open PersonalBot.Shared.Domain.Weather.Utils

type YandexWeatherProvider = HtmlProvider<"./data/yandex.ru_pogoda_samara_details.html">
type TodayWeatherSummary = { 
    Explanation: string;
    ExplanationRain: string;
    Temperature: string; 
    DetailsUri: string;
    RainMapUri: string
   }

let hostUri = "https://yandex.ru"
let weatherUri = "https://yandex.ru/pogoda/samara/details"

let getTodaySummaryAsync = 
    async {
        let! page = YandexWeatherProvider.AsyncLoad weatherUri

        let selector = HtmlSelector page.Html
            
        let degrees = selector.SelectNodeText (CssSelector ".current-weather__thermometer_type_now")
        let explanation = selector.SelectNodeText (CssSelector ".current-weather__comment")
        let explanationRain = 
            selector.SelectNodeText (CssSelectors [".nowcast-switcher__title_long > span"; ".nowcast-switcher__title_long"])

        let getDefaulValue opt = defaultArg opt "Unknown"

        return { 
            Explanation = getDefaulValue explanation;
            ExplanationRain = getDefaulValue explanationRain;
            Temperature = getDefaulValue degrees; 
            DetailsUri = weatherUri;
            RainMapUri = "https://yandex.ru/pogoda/samara/nowcast"
        }
    }