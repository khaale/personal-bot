namespace PersonalBot.Shared.Domain.Weather

open Microsoft.Bot.Connector
open PersonalBot.Shared.Core.Responders
open PersonalBot.Shared.Domain.Weather.WeatherResponderImpl
 
type WeatherResponder(sender: IMessageSender) =

    let env = {
        new Env.IEnvironment with
            member this.GetRamblerWeatherAsync () = RamblerWeather.getTodaySummaryAsync
            member this.GetYandexWeatherAsync () = YandexWeather.getTodaySummaryAsync
            member this.SendReplyAsync x = sender.SendAsync x |> Async.AwaitTask
    }
    
    member this.ProcessAsync (reply: IMessageActivity) = 
        sendWeather reply env |> Async.StartAsTask
