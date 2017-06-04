// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "../packages/FSharp.Data.2.3.1/lib/net40/FSharp.Data.dll"
#load "Utils.fs"

open FSharp.Data
open PersonalBot.Shared.Domain.Weather.Utils

let ramblerWeatherUri = "https://weather.rambler.ru/v-samare/"

type RamblerWeather = HtmlProvider<"./data/weather.rambler.ru.html">

type YandexWeather = HtmlProvider<"https://yandex.ru/pogoda/samara/details">

let yw = YandexWeather.Load "https://yandex.ru/pogoda/samara/details"

let selector = new HtmlSelector(yw.Html)

let degrees = selector.SelectNodeText (CssSelector ".current-weather__thermometer_type_now")
let explanation = selector.SelectNodeText (CssSelector ".current-weather__comment")
let explanationRain = 
    selector.SelectNodeText (CssSelectors [".nowcast-switcher__title_long > span"; ".nowcast-switcher__title_long"])
