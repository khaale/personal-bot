namespace PersonalBot.Shared.Domain.Torrents.Responders
{
    public class Action
    {
        public Action(string command, string caption = null, string description = null)
        {
            Command = command;
            Caption = caption;
            Description = description;
        }

        public string Command { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
    }
}
