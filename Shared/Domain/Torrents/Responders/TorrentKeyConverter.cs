using PersonalBot.Shared.Core.Responders;
using PersonalBot.Shared.Domain.Torrents.Models;

namespace PersonalBot.Shared.Domain.Torrents.Responders
{
    public class TorrentKeyConverter : IArgumentsConverter<TorrentKey>
    {
        public TorrentKey FromMessage(string message)
        {
            return new TorrentKey(message);
        }

        public string ToMessage(TorrentKey args)
        {
            return args.ToString();
        }
    }
}