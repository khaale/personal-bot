using System;
using System.Threading.Tasks;

namespace PersonalBot.Shared.Core.Responders
{

    public class Action<TArg,TConverter> where TConverter: class, IArgumentsConverter<TArg>, new()
    {
        private readonly TConverter _converter;

        public Action(string prefix, string command, string caption = null, string description = null, TConverter converter = null)
        {
            Prefix = prefix;
            Command = command;
            Caption = caption;
            Description = description;
            _converter = converter ?? new TConverter();
        }

        protected string Prefix { get; set; }
        protected string Command { get; set; }

        public string Caption { get; set; }
        public string Description { get; set; }

        protected string FullCommand => $"{Prefix} {Command}".Trim();

        public virtual bool Match(string text, out TArg args)
        {
            var matched = text != null && text.StartsWith(FullCommand);

            args = matched
                ? _converter.FromMessage(text.Remove(0, FullCommand.Length).Trim())
                : default(TArg);

            return matched;
        }
        
        public virtual Task<bool> MatchAndDoAsync(string text, Func<TArg,Task> action)
        {
            return Match(text, out var args)
                ? action(args).ContinueWith(_ => true)
                : Task.FromResult(false);
        }

        public string ToMessage(TArg arg)
        {
            return $"{FullCommand} {arg}".Trim();
        }
    }
}
