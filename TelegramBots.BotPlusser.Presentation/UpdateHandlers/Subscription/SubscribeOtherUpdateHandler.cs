using TelegramBots.BotPlusser.Application.Base;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Subscription;

[AllowedUpdateType(UpdateType.Message, @"^/\+ (?<memberName>.+)$")]
public class SubscribeOtherUpdateHandler : UpdateHandler
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscribeOtherUpdateHandler(
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

        var memberName = Arguments["memberName"];
        var groupTelegramId = message.Chat.Id;

        await _subscriptionService.SubscribeToGroupGathering(groupTelegramId, memberName);
    }
}