namespace PersonalBot.Shared.Domain.Weather

open Microsoft.Bot.Connector
open PersonalBot.Shared.Core.Responders
open PersonalBot.Shared.Domain.Weather.WeatherResponderImpl
 
type WeatherResponder(sender: IMessageSender) =

    let envImpl = {
        new Env.IEnvironment with
            member this.GetRamblerWeather () = RamblerWeather.getTodaySummaryAsync
            member this.GetYandexWeather () = YandexWeather.getTodaySummaryAsync
            member this.SendReply x = sender.SendAsync x |> Async.AwaitTask
    }
    
    member this.ProcessAsync (reply: IMessageActivity) = 
        sendWeather reply envImpl |> Async.StartAsTask
