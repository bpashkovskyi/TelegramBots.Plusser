using TelegramBots.BotPlusser.Domain.Model.Enums;

namespace TelegramBots.BotPlusser.Domain.Model.ValueObjects;

public class UnSubscriptionResult
{
    private readonly string _eventName;
    private readonly UnSubscriptionStatus _unSubscriptionStatus;

    public UnSubscriptionResult(UnSubscriptionStatus unSubscriptionStatus, string eventName)
    {
        _unSubscriptionStatus = unSubscriptionStatus;
        _eventName = eventName;
    }

    public string Message
    {
        get
        {
            var message = _unSubscriptionStatus switch
            {
                UnSubscriptionStatus.UnSubscriptionConfirmed => $"Мінус на подію \"{_eventName}\" зараховано",
                UnSubscriptionStatus.AlreadyUnsubscribed => $"Ви вже відмовилися від події \"{_eventName}\"",
                _ => throw new NotSupportedException(_unSubscriptionStatus.ToString())
            };

            return message;
        }
    }
}