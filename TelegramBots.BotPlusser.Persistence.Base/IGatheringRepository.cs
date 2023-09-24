using TelegramBots.BotPlusser.Domain.Model.Entities;

namespace TelegramBots.BotPlusser.Persistence.Base;

public interface IGatheringRepository
{
    Task<Gathering?> GetNonDraftGatheringCreatedByAsync(long creatorTelegramId);

    Task<Gathering> GetGatheringAsync(int id);

    Task AddAsync(Gathering gathering);

    void Update(Gathering gathering);

    void Delete(Gathering gathering);

    Task SaveChangesAsync();
}