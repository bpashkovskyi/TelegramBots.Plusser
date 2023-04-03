namespace TelegramBots.BotPlusser.Presentation.UpdateHandlers.Subscription;

[AllowedUpdateType(UpdateType.Message)]
[AllowedChats(-930667525)]
public class WriteToTtkMessageHandler : UpdateHandler
{
    public WriteToTtkMessageHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient)
        : base(rollbar, telegramBotClient)
    {
    }

    public override async Task HandleAsync(Update update)
    {
        if (update.Message!.From!.Id != AppSettings.BoId)
        {
            return;
        }

        await TelegramBotClient.SendTextMessageAsync(-1001392138663, update.Message!.Text!);
    }
}