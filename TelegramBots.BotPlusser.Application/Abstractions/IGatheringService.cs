namespace TelegramBots.BotPlusser.Application.Abstractions;

public interface IGatheringService
{
    Task GetNextGatheringPropertyMessageAsync(long creatorTelegramId, string propertyValue);

    Task CreateGatheringAsync(long creatorTelegramId, long groupTelegramId);

    Task DeleteGatheringAsync(int gatheringId);

    Task DeleteGroupGatheringAsync(long groupTelegramId);

    Task RefreshGroupGroupGatheringsAsync(long groupTelegramId);

    Task BroadcastMessageAsync(string messageText);
}