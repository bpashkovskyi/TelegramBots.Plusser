using TelegramBots.BotPlusser.Domain.Model.Entities;

namespace TelegramBots.BotPlusser.Persistence.Base;

public interface IGroupRepository
{
    Task<Group?> GetGroupAsync(long telegramId);

    Task<Group> GetOrCreateGroupAsync(long telegramId);

    Task<List<long>> GetGroupsTelegramIdsAsync();
}