using System.Diagnostics;
using PersonalBot.Shared.Core.Responders;
using PersonalBot.Shared.Domain.Torrents.Models;
using PersonalBot.Shared.Domain.Torrents.Responders;

namespace PersonalBot.Shared.Domain
{
    public class TorrentAction: Action<TorrentKey, TorrentKeyConverter>
    {
        public TorrentAction(string prefix, string command, string caption, string description, bool hidden = true) 
            : base(prefix, command, caption, description, hidden: hidden)
        {
        }
    }

    public static class Actions
    {
        public static readonly SimpleAction Help = new SimpleAction("!help", "", "Help", "See all available commands");

        public static readonly string WeatherPrefix = "!w";
        public static readonly SimpleAction Weather = new SimpleAction(WeatherPrefix, "", "Weather", "Shows weather");

        public static readonly string TorrentsPrefix = "!t";
        public static readonly SimpleAction TorrentList = new SimpleAction(TorrentsPrefix, "", "List", "See torrents list");
        public static readonly TorrentAction TorrentSeen = new TorrentAction(TorrentsPrefix, "seen", "Mark as seen", "Marks torrent as seen");
        public static readonly TorrentAction TorrrentDownloaded = new TorrentAction(TorrentsPrefix, "downloaded", "Mark as downloaded", "Marks torrent as downloaded");
        public static readonly TorrentAction TorrentResetState = new TorrentAction(TorrentsPrefix, "reset state", "Reset state", "Resets torrent state");
        public static readonly SimpleAction TorrentSubscribe = new SimpleAction(TorrentsPrefix, "subscribe", "Subscribe", "Subscribes on new torrent announcements");
    }
}