using TelegramBots.BotPlusser.Application.Abstractions;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Gatherings;

[AllowedUpdateType(UpdateType.Message, "^/broadcast (?<message>.+)$")]
[AllowedChats(AppSettings.BoId)]
public class BroadcastMessageUpdateHandler : UpdateHandler
{
    private readonly IGatheringService _gatheringService;

    public BroadcastMessageUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IGatheringService gatheringService)
        : base(rollbar, telegramBotClient)
    {
        _gatheringService = gatheringService;
    }

    public override async Task HandleAsync(Update update)
    {
        var messageText = Arguments["message"];

        await _gatheringService.BroadcastMessageAsync(messageText);
    }
}