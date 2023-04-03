using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Domain.Abstractions;

public interface IGatheringRepository
{
    Task<Gathering?> GetNonDraftGatheringsByCreatorAsync(long creatorTelegramId);

    Task<Gathering> GetGatheringAsync(int id);

    Task AddAsync(Gathering gathering);

    void Update(Gathering gathering);

    void Delete(Gathering gathering);

    Task SaveChangesAsync();
}