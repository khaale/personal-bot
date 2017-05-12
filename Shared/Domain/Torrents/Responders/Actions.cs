namespace PersonalBot.Shared.Domain.Torrents.Responders
{
    public static class Actions
    {
        public static Action TorrrentSeen = new Action("!t seen", "Mark as seen", "Marks torrent as seen");
        public static Action TorrrentDownloaded = new Action("!t downloaded", "Mark as downloaded", "Marks torrent as downloaded");
        public static Action TorrentResetState = new Action("!t reset state", "Reset state", "Resets torrent state");
        public static Action TorrentSubscribe = new Action("!t subscribe", "Subscribe", "Subscribes on new torrent announcements");
    }
}