using TelegramBots.BotPlusser.Application.Abstractions;
using TelegramBots.Shared.Extensions;

namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Gatherings;

[AllowedUpdateType(UpdateType.Message, "^/new")]
public class CreateGatheringUpdateHandler : UpdateHandler
{
    private readonly IGatheringService _gatheringService;

    public CreateGatheringUpdateHandler(
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

        try
        {
            if (memberInfo.Status is ChatMemberStatus.Administrator or ChatMemberStatus.Creator)
            {
                var creatorTelegramId = message.Chat.Id;
                var groupTelegramId = message.Chat.Id;

                await _gatheringService.CreateGatheringAsync(creatorTelegramId, groupTelegramId);
            }
            else
            {
                await TelegramBotClient.SendTextMessageAsync(message.From.Id, "Вам потрібні права адміністратора");
            }
        }
        catch (Exception exception)
        {
            Rollbar.Error(exception);
            await TelegramBotClient.SendTextMessageAsync(message.Chat.Id,
                "Бот не може розпочати з вами приватний діалог. Додайте бота у контакти (https://t.me/plusser_bot), натисніть /start та спробуйте команду /new у чаті знову.");
        }
    }
}