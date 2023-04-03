using TelegramBots.BotPlusser.Application.Abstractions;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.SubscriptionCallback;

[AllowedUpdateType(UpdateType.CallbackQuery, "^subscribeme;(?<memberId>.+);(?<gatheringId>.+)$")]
public class SubscribeMeCallbackUpdateHandler : UpdateHandler
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscribeMeCallbackUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        ISubscriptionService subscriptionService)
        : base(rollbar, telegramBotClient)
    {
        _subscriptionService = subscriptionService;
    }

    public override async Task HandleAsync(Update update)
    {
        var message = update.CallbackQuery!.Message!;

        var memberTelegramId = int.Parse(Arguments["memberId"], CultureInfo.InvariantCulture);
        var gatheringId = int.Parse(Arguments["gatheringId"], CultureInfo.InvariantCulture);

        await _subscriptionService.SubscribeToGathering(gatheringId, memberTelegramId);

        await TelegramBotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
    }
}