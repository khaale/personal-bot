using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PersonalBot.Shared.Domain.Torrents.Models
{
    public class TorrentPresenter
    {
        private static readonly Regex SeriesRegex =
            new Regex(@"Серии([^(]+)(\(\d+\))?", RegexOptions.Compiled);

        public string Title { get; private set; }
        public string Series { get; set; }
        public string Updated { get; set; }
        public string TopicUrl { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsSeen { get; set; }
        public bool IsDownloaded { get; set; }

        public TorrentKey Key { get; set; }

        public TorrentPresenter(Topic topic, TorrentEntity entity = null)
        {
            Title = topic.TopicTitle.Split('/').FirstOrDefault()?.Trim() ?? topic.TopicTitle;

            var seriesMatch = SeriesRegex.Match(topic.TopicTitle);
            Series = seriesMatch.Success
                ? seriesMatch.Value.Trim()
                : "Серии: нет данных";

            var timeSinceUpdate = DateTime.Now - topic.RegTime;

            if (timeSinceUpdate.TotalHours > 1)
            {
                var sb = new StringBuilder();
                if (timeSinceUpdate.TotalDays >= 1)
                {
                    sb.AppendFormat(
                        "{0} day{1}", 
                        (int)timeSinceUpdate.TotalDays, 
                        timeSinceUpdate.TotalDays >= 2 ? "s" : String.Empty);
                }
                if (timeSinceUpdate.TotalDays >= 1 && timeSinceUpdate.Hours >= 1)
                {
                    sb.Append(", ");
                }
                if (timeSinceUpdate.Hours >= 1)
                {
                    sb.AppendFormat(
                        "{0} hour{1}", 
                        timeSinceUpdate.Hours,
                        timeSinceUpdate.Hours >=  2 ? "s" : String.Empty);
                }
                sb.Append(" ago");
                Updated = sb.ToString();
            }
            else
            {
                Updated = "now";
            }

            TopicUrl = $"https://rutracker.org/forum/viewtopic.php?t={topic.Id}";
            DownloadUrl = $"https://rutracker.org/forum/dl.php?t={topic.Id}";
            Key = new TorrentKey(topic.Id, topic.InfoHash);
            IsSeen = entity?.IsSeen ?? false;
            IsDownloaded = entity?.IsDownloaded ?? false;
        }

        private static string GetIntervalAsString(DateTime time)
        {
            var timeSinceUpdate = DateTime.Now - time;

            if (timeSinceUpdate.TotalHours > 1)
            {
                var sb = new StringBuilder();
                if (timeSinceUpdate.TotalDays >= 1)
                {
                    sb.AppendFormat(
                        "{0} day{1}",
                        (int)timeSinceUpdate.TotalDays,
                        timeSinceUpdate.TotalDays >= 2 ? "s" : String.Empty);
                }
                if (timeSinceUpdate.TotalDays >= 1 && timeSinceUpdate.Hours >= 1)
                {
                    sb.Append(", ");
                }
                if (timeSinceUpdate.Hours >= 1)
                {
                    sb.AppendFormat(
                        "{0} hour{1}",
                        timeSinceUpdate.Hours,
                        timeSinceUpdate.Hours >= 2 ? "s" : String.Empty);
                }
                sb.Append(" ago");
                return sb.ToString();
            }
            else
            {
                return "now";
            }
        }
    }
}
