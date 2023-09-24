using TelegramBots.BotPlusser.Application.Base;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Subscription;

[AllowedUpdateType(UpdateType.Message, @"^/\-$")]
public class UnSubscribeMeUpdateHandler : UpdateHandler
{
    private readonly ISubscriptionService _subscriptionService;

    public UnSubscribeMeUpdateHandler(
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
        var memberTelegramId = message.From!.Id;

        await _subscriptionService.UnsubscribeFromGroupGathering(groupTelegramId, memberTelegramId);
    }
}