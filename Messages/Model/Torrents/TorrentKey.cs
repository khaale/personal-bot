using System;

namespace Messages.Model.Torrents
{
    public class TorrentKey : IEquatable<TorrentKey>
    {
        public string TopicId { get; set; }
        public string InfoHash { get; set; }

        public TorrentKey(string topicId, string infoHash)
        {
            TopicId = topicId;
            InfoHash = infoHash;
        }


        public TorrentKey(string key)
        {
            var tokens = key.Split('-');
            TopicId = tokens[0];
            InfoHash = tokens[1];
        }

        public override string ToString()
        {
            return $"{TopicId}-{InfoHash}";
        }

        public override bool Equals(object other)
        {
            if (other == null) { return false; }
            if (object.ReferenceEquals(this, other)) { return true; }

            return Equals(other as TorrentKey);
        }

        public bool Equals(TorrentKey other)
        {
            if (other == null)
            {
                return false;
            }

            return
                TopicId == other.TopicId
                && InfoHash == other.InfoHash;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
