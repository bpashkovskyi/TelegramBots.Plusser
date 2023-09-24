namespace TelegramBots.BotPlusser.Application.Base;

public interface ISubscriptionService
{
    Task SubscribeToGathering(int gatheringId, long memberTelegramId);
    Task SubscribeToGathering(int gatheringId, string memberName);

    Task SubscribeToGroupGathering(long groupTelegramId, long memberTelegramId);
    Task SubscribeToGroupGathering(long groupTelegramId, string memberName);

    Task UnsubscribeFromGathering(int gatheringId, long memberTelegramId);
    Task UnsubscribeFromGathering(int gatheringId, string memberName);

    Task UnsubscribeFromGroupGathering(long groupTelegramId, long memberTelegramId);
    Task UnsubscribeFromGroupGathering(long groupTelegramId, string memberName);
}