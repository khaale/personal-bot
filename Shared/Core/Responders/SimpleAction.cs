namespace PersonalBot.Shared.Core.Responders
{
    public class SimpleAction : Action<DummyArgs, DummyConverter>
    {
        public SimpleAction(string prefix, string command, string caption, string description)
            : base(prefix, command, caption, description)
        {
        }
    }

    public class DummyConverter : IArgumentsConverter<DummyArgs>
    {
        public DummyArgs FromMessage(string message)
        {
            return new DummyArgs();
        }

        public string ToMessage(DummyArgs args)
        {
            return string.Empty;
        }
    }

    public class DummyArgs
    {
    }
}