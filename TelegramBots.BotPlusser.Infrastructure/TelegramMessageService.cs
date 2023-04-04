using Telegram.Bot.Types.ReplyMarkups;

using TelegramBots.BotPlusser.Domain.Abstractions;
using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Infrastructure;

public class TelegramMessageService : ITelegramMessageService
{
    private readonly IRollbar _rollbar;
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramMessageService(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient)
    {
        _rollbar = rollbar;
        _telegramBotClient = telegramBotClient;
    }

    public async Task SendGatheringChooseMessageAsync(Group group, string callbackMessage)
    {
        var buttons = group.NonDraftGatherings.Select(
            @event => new InlineKeyboardButton(@event.Name!)
            {
                CallbackData = $"{callbackMessage};{@event.Id}"
            }).ToList();

        var markup = new InlineKeyboardMarkup(buttons);

        await _telegramBotClient.SendTextMessageAsync(group.TelegramId, "Оберіть подію:", replyMarkup: markup);
    }

    public async Task SendTextMessageAsync(long chatTelegramId, string messageText)
    {
        try
        {
            await _telegramBotClient.SendTextMessageAsync(chatTelegramId, messageText);
        }
        catch (Exception exception)
        {
            _rollbar.Error(exception);
        }
    }

    public async Task SendGatheringCreatedMessageAsync(Gathering gathering)
    {
        await _telegramBotClient.SendTextMessageAsync(gathering.Group!.TelegramId,
            $"Подію \"{gathering.Name}\" створено.");
    }

    public async Task SendNonCompletedGatheringCreationMessageAsync(long creatorTelegramId)
    {
        await _telegramBotClient.SendTextMessageAsync(creatorTelegramId,
            "Ви не закінчили створення попередньої події.");
    }

    public async Task SendErrorMessageAsync(long creatorTelegramId)
    {
        await _telegramBotClient.SendTextMessageAsync(creatorTelegramId, "Трапилася помилка");
    }

    public async Task SendUnsubscribeGatheringChooseMessageAsync(long memberTelegramId, Group group)
    {
        var callbackMessage = $"unsubscribeme;{memberTelegramId}";
        await SendGatheringChooseMessageAsync(group, callbackMessage);
    }

    public async Task SendUnsubscribeGatheringChooseMessageAsync(string memberName, Group group)
    {
        var callbackMessage = $"unsubscribe;{memberName}";
        await SendGatheringChooseMessageAsync(group, callbackMessage);
    }

    public async Task SendSubscribeGatheringChooseMessageAsync(long memberTelegramId, Group group)
    {
        var callbackMessage = $"subscribeme;{memberTelegramId}";
        await SendGatheringChooseMessageAsync(group, callbackMessage);
    }

    public async Task SendSubscribeGatheringChooseMessage(string memberName, Group group)
    {
        var callbackMessage = $"subscribe;{memberName}";
        await SendGatheringChooseMessageAsync(group, callbackMessage);
    }

    public async Task UpdateGatheringChatPinnedMessageAsync(Gathering gathering)
    {
        var eventText = gathering.GetAttendanceInformation();
        var chat = gathering.Group!;

        int pinnedMessageId;

        // Event pinned
        if (gathering.PinnedMessageId.HasValue)
        {
            try
            {
                var pinnedMessage = await _telegramBotClient.EditMessageTextAsync(chat.TelegramId, gathering.PinnedMessageId.Value, eventText);
                pinnedMessageId = pinnedMessage.MessageId;
            }
            catch
            {
                // Text is the same, or message not exists
                try
                {
                    await _telegramBotClient.UnpinChatMessageAsync(chat.TelegramId, gathering.PinnedMessageId.Value);
                }
                catch
                {
                    _rollbar.Error($"Cannot unpin message {gathering.PinnedMessageId.Value} in chat {chat.TelegramId}");
                }

                var pinnedMessage = await _telegramBotClient.SendTextMessageAsync(chat.TelegramId, eventText);
                await _telegramBotClient.PinChatMessageAsync(chat.TelegramId, pinnedMessage.MessageId, true);

                pinnedMessageId = pinnedMessage.MessageId;
            }
        }
        else
        {
            var pinnedMessage = await _telegramBotClient.SendTextMessageAsync(chat.TelegramId, eventText);
            await _telegramBotClient.PinChatMessageAsync(chat.TelegramId, pinnedMessage.MessageId, true);

            pinnedMessageId = pinnedMessage.MessageId;
        }

        gathering.PinnedMessageId = pinnedMessageId;
    }

    public async Task UnpinChatMessageAsync(Gathering gathering)
    {
        await _telegramBotClient.UnpinChatMessageAsync(gathering.Group!.TelegramId, gathering.PinnedMessageId);
    }
}