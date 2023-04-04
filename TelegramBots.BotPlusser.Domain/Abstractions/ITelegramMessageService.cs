using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Domain.Abstractions;

public interface ITelegramMessageService
{
    Task SendGatheringChooseMessageAsync(Group group, string callbackMessage);

    Task SendTextMessageAsync(long chatTelegramId, string messageText);
    Task SendGatheringCreatedMessageAsync(Gathering gathering);
    Task SendNonCompletedGatheringCreationMessageAsync(long creatorTelegramId);
    Task SendErrorMessageAsync(long creatorTelegramId);

    Task SendSubscribeGatheringChooseMessageAsync(long memberTelegramId, Group group);
    Task SendSubscribeGatheringChooseMessage(string memberName, Group group);
    Task SendUnsubscribeGatheringChooseMessageAsync(long memberTelegramId, Group group);
    Task SendUnsubscribeGatheringChooseMessageAsync(string memberName, Group group);

    Task UpdateGatheringChatPinnedMessageAsync(Gathering gathering);
    Task UnpinChatMessageAsync(Gathering gathering);
}