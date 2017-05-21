namespace PersonalBot.Shared.Core.Responders
{
    public interface IArgumentsConverter<T>
    {
        T FromMessage(string source);

        string ToMessage(T converter);
    }
}