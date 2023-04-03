namespace TelegramBots.BotPlusser.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class MessageAttribute : Attribute
{
    public MessageAttribute(string text, int order = int.MaxValue)
    {
        Text = text;
        Order = order;
    }

    public string Text { get; }

    public int Order { get; }
}