using TelegramBots.BotPlusser.Application.Abstractions;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Gatherings;

[AllowedUpdateType(UpdateType.Message)]
[MessageShouldNotBeCommand]
[AllowedChatTypes(ChatType.Private)]
public class FillGatheringPropertyUpdateHandler : UpdateHandler
{
    private readonly IGatheringService _gatheringService;

    public FillGatheringPropertyUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IGatheringService gatheringService)
        : base(rollbar, telegramBotClient)
    {
        _gatheringService = gatheringService;
    }

    public override async Task HandleAsync(Update update)
    {
        var message = update.Message!;

        try
        {
            var creatorTelegramId = message.From!.Id;
            var propertyValue = message.Text!;

            await _gatheringService.GetNextGatheringPropertyMessage(creatorTelegramId, propertyValue);
        }
        catch (FormatException formatException)
        {
            Rollbar.Error(formatException);
            await TelegramBotClient.SendTextMessageAsync(message.Chat.Id, "Сталася помилка, спробуйте знову.");
        }
    }
}