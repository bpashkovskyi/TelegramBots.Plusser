using TelegramBots.BotPlusser.Application.Abstractions;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Gatherings;

[AllowedUpdateType(UpdateType.CallbackQuery, @"^deletegathering;(?<gatheringId>\d*)$")]
public class DeleteGatheringCallbackUpdateHandler : UpdateHandler
{
    private readonly IGatheringService _gatheringService;

    public DeleteGatheringCallbackUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IGatheringService gatheringService)
        : base(rollbar, telegramBotClient)
    {
        _gatheringService = gatheringService;
    }

    public override async Task HandleAsync(Update update)
    {
        var message = update.CallbackQuery!.Message!;

        var gatheringId = int.Parse(Arguments["gatheringId"], CultureInfo.InvariantCulture);

        await _gatheringService.DeleteGathering(gatheringId);

        await TelegramBotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
    }
}