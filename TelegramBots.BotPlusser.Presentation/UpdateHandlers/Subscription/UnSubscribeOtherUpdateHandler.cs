using TelegramBots.BotPlusser.Application.Base;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Subscription;

[AllowedUpdateType(UpdateType.Message, @"^/\- (?<memberName>.+)$")]
public class UnSubscribeOtherUpdateHandler : UpdateHandler
{
    private readonly ISubscriptionService _subscriptionService;

    public UnSubscribeOtherUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        ISubscriptionService subscriptionService)
        : base(rollbar, telegramBotClient)
    {
        _subscriptionService = subscriptionService;
    }

    public override async Task HandleAsync(Update update)
    {
        var message = update.Message!;

        var groupTelegramId = message.Chat.Id;
        var memberName = Arguments["memberName"];

        await _subscriptionService.UnsubscribeFromGroupGathering(groupTelegramId, memberName);
    }
}