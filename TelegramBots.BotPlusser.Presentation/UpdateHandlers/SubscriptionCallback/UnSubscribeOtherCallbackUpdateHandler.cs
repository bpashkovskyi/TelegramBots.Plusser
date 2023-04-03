using TelegramBots.BotPlusser.Application.Abstractions;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.SubscriptionCallback;

[AllowedUpdateType(UpdateType.CallbackQuery, "^unsubscribe;(?<memberName>.+);(?<gatheringId>.+)$")]
public class UnSubscribeOtherCallbackUpdateHandler : UpdateHandler
{
    private readonly ISubscriptionService _subscriptionService;

    public UnSubscribeOtherCallbackUpdateHandler(
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

        await _subscriptionService.UnsubscribeFromGathering(gatheringId, memberName);

        await TelegramBotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
    }
}