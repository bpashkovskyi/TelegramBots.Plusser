using Microsoft.EntityFrameworkCore;

using TelegramBots.BotPlusser.Domain.Abstractions;
using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Infrastructure;

public class GatheringRepository : IGatheringRepository
{
    private readonly PlusserContext _plusserContext;

    public GatheringRepository(PlusserContext plusserContext)
    {
        _plusserContext = plusserContext;
    }

    public async Task<Gathering?> GetNonDraftGatheringCreatedByAsync(long creatorTelegramId)
    {
        return await _plusserContext.Gatherings
            .Include(gathering => gathering.Group)
            .Where(gathering => gathering.Creator!.TelegramId == creatorTelegramId)
            .Where(currentEvent => currentEvent.IsDraft)
            .OrderByDescending(currentEvent => currentEvent.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<Gathering> GetGatheringAsync(int id)
    {
        return await _plusserContext.Gatherings
            .Include(currentEvent => currentEvent.Group)
            .Include(currentEvent => currentEvent.Attendees)
            .ThenInclude(currentAttendee => currentAttendee.Member)
            .FirstAsync(currentEvent => currentEvent.Id == id);
    }

    public async Task AddAsync(Gathering gathering)
    {
        await _plusserContext.AddAsync(gathering);
    }

    public void Update(Gathering gathering)
    {
        _plusserContext.Update(gathering);
    }

    public void Delete(Gathering gathering)
    {
        _plusserContext.Remove(gathering);
    }

    public async Task SaveChangesAsync()
    {
        await _plusserContext.SaveChangesAsync();
    }
}