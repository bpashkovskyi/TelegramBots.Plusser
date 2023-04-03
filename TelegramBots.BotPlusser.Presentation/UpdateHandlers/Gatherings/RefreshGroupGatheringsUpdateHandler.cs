using TelegramBots.BotPlusser.Application.Abstractions;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Gatherings;

[AllowedUpdateType(UpdateType.Message, "^/refresh")]
public class RefreshGroupGatheringsUpdateHandler : UpdateHandler
{
    private readonly IGatheringService _gatheringService;

    public RefreshGroupGatheringsUpdateHandler(
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

        var memberInfo = await TelegramBotClient.GetChatMemberAsync(message.Chat.Id, message.From!.Id);
        if (memberInfo.Status is ChatMemberStatus.Administrator or ChatMemberStatus.Creator)
        {
            var groupTelegramId = message.Chat.Id;
            await _gatheringService.RefreshGroupGroupGatherings(groupTelegramId);
        }
        else
        {
            await TelegramBotClient.SendTextMessageAsync(message.From!.Id, "Вам потрібні права адміністратора");
        }
    }
}