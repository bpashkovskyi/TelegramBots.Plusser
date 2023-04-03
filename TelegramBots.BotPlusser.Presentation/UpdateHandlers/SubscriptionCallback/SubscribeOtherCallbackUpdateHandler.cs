using TelegramBots.BotPlusser.Application.Abstractions;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.SubscriptionCallback;

[AllowedUpdateType(UpdateType.CallbackQuery, "^subscribe;(?<memberName>.+);(?<gatheringId>.+)$")]
public class SubscribeOtherCallbackUpdateHandler : UpdateHandler
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscribeOtherCallbackUpdateHandler(
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

        var memberName = Arguments["memberName"];
        var gatheringId = int.Parse(Arguments["gatheringId"], CultureInfo.InvariantCulture);

        await _subscriptionService.SubscribeToGathering(gatheringId, memberName);

        await TelegramBotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
    }
}