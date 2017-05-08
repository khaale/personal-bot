using System;

namespace Messages.Model.Torrents
{
    public class Topic
    {
        private static readonly System.DateTime unixEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        public string Id { get; set; }
        public string InfoHash { get; set; }
        public DateTime RegTime { get; set; }
        public string TopicTitle { get; set; }

        public Topic()
        {

        }

        public Topic(string id, string infoHash, int regTime, string topicTitle)
        {
            Id = id;
            InfoHash = infoHash;
            RegTime = unixEpochStart.AddSeconds(regTime);
            TopicTitle = topicTitle;
        }
        
        public TorrentKey GetKey()
        {
            return new TorrentKey(Id, InfoHash);
        }
    }
}
