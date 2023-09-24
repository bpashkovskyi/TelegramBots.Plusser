using TelegramBots.BotPlusser.Domain.Model.Enums;

namespace TelegramBots.BotPlusser.Domain.Model.ValueObjects;

public class SubscriptionResult
{
    private readonly string _gatheringName;
    private readonly SubscriptionStatus _subscriptionStatus;

    public SubscriptionResult(SubscriptionStatus subscriptionStatus, string gatheringName)
    {
        _subscriptionStatus = subscriptionStatus;
        _gatheringName = gatheringName;
    }

    public string Message
    {
        get
        {
            var message = _subscriptionStatus switch
            {
                SubscriptionStatus.AlreadySubscribed => $"Ви вже зареєстровані на подію \"{_gatheringName}\"",
                SubscriptionStatus.SubscriptionConfirmed => $"Плюс на подію \"{_gatheringName}\" зараховано",
                SubscriptionStatus.NoPlace => $"Уже немає місця на подію \"{_gatheringName}\"",
                _ => throw new NotSupportedException(_subscriptionStatus.ToString())
            };

            return message;
        }
    }
}