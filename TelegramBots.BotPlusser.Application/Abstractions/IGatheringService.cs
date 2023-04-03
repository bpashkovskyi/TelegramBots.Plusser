namespace TelegramBots.BotPlusser.Application.Abstractions;

public interface IGatheringService
{
    Task GetNextGatheringPropertyMessage(long creatorTelegramId, string propertyValue);

    Task CreateGathering(long creatorTelegramId, long groupTelegramId);

    Task DeleteGathering(int gatheringId);

    Task DeleteGroupGathering(long groupTelegramId);

    Task RefreshGroupGroupGatherings(long groupTelegramId);

    Task BroadcastMessage(string messageText);
}