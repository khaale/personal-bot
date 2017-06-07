#r "../Shared/bin/Debug/PersonalBot.Shared.dll"
#r "../packages/Microsoft.Bot.Builder.3.5.5/lib/net46/Microsoft.Bot.Builder.dll"
#r "../packages/Microsoft.Bot.Builder.3.5.5/lib/net46/Microsoft.Bot.Connector.dll"
#r "../packages/FSharp.Data.2.3.1/lib/net40/FSharp.Data.dll"
#r "../packages/Newtonsoft.Json.9.0.1/lib/net45/Newtonsoft.Json.dll"
#r "../packages/Microsoft.Rest.ClientRuntime.2.3.2/lib/net45/Microsoft.Rest.ClientRuntime.dll"
#load "./Utils.fs"
#load "./RamblerWeather.fs"
#load "./YandexWeather.fs"
#load "./WeatherResponderImpl.fs"

open Newtonsoft.Json

open Microsoft.Bot.Connector
open PersonalBot.Shared.Domain.Weather
open PersonalBot.Shared.Domain.Weather.WeatherResponderImpl

let prettyPrint x = 
    let ser = new JsonSerializerSettings ()
    ser.NullValueHandling <- NullValueHandling.Ignore
    JsonConvert.SerializeObject (x,Formatting.Indented, ser)

let msg = (new Activity()) :> IMessageActivity
let fakeEnv = {
    new Env.IEnvironment with
        member this.GetRamblerWeather () = RamblerWeather.getTodaySummaryAsync
        member this.GetYandexWeather () = YandexWeather.getTodaySummaryAsync
        member this.SendReply x = async { printfn "%s" <| prettyPrint x }
    }

sendWeather msg fakeEnv |> Async.RunSynchronously |> ignore
