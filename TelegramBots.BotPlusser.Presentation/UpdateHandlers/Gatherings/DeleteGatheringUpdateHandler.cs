using TelegramBots.BotPlusser.Application.Base;
using TelegramBots.Shared.Extensions;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Gatherings;

[MessageShouldBeCommand("delete")]
public class DeleteGatheringUpdateHandler : UpdateHandler
{
    private readonly IGatheringService _gatheringService;

    public DeleteGatheringUpdateHandler(
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

        Rollbar.Info(message.GetSenderInfo());

        var memberInfo = await TelegramBotClient.GetChatMemberAsync(message.Chat.Id, message.From!.Id);
        if (memberInfo.Status is ChatMemberStatus.Administrator or ChatMemberStatus.Creator)
        {
            var groupTelegramId = message.Chat.Id;

            await _gatheringService.DeleteGroupGatheringAsync(groupTelegramId);

            await TelegramBotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        }
        else
        {
            await TelegramBotClient.SendTextMessageAsync(message.From.Id, "Вам потрібні права адміністратора");
        }
    }
}