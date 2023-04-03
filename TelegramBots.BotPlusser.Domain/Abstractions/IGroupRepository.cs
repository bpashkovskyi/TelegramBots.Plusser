using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Domain.Abstractions;

public interface IGroupRepository
{
    Task<Group?> GetGroupAsync(long telegramId);

    Task<Group> GetOrCreateGroupAsync(long telegramId);

    Task<List<long>> GetGroupsTelegramIdsAsync();
}