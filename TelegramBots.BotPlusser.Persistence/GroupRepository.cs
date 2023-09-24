using Microsoft.EntityFrameworkCore;

using Telegram.Bot;

using TelegramBots.BotPlusser.Domain.Model.Entities;
using TelegramBots.BotPlusser.Persistence.Base;

namespace TelegramBots.BotPlusser.Persistence;

public class GroupRepository : IGroupRepository
{
    private readonly PlusserContext _plusserContext;
    private readonly ITelegramBotClient _telegramBotClient;

    public GroupRepository(
        PlusserContext plusserContext,
        ITelegramBotClient telegramBotClient)
    {
        _plusserContext = plusserContext;
        _telegramBotClient = telegramBotClient;
    }

    public async Task<Group?> GetGroupAsync(long telegramId)
    {
        return await _plusserContext.Groups
            .Include(group => group.Gatherings)
            .ThenInclude(gathering => gathering.Attendees)
            .ThenInclude(attendee => attendee.Member)
            .FirstOrDefaultAsync(group => group.TelegramId == telegramId);
    }

    public async Task<Group> GetOrCreateGroupAsync(long telegramId)
    {
        var group = await _plusserContext.Groups
            .Include(group => group.Gatherings)
            .ThenInclude(gathering => gathering.Attendees)
            .ThenInclude(attendee => attendee.Member)
            .FirstOrDefaultAsync(group => group.TelegramId == telegramId);

        if (group is null)
        {
            var telegramChat = await _telegramBotClient.GetChatAsync(telegramId);
            group = new Group(telegramChat);
        }

        return group;
    }

    public async Task<List<long>> GetGroupsTelegramIdsAsync()
    {
        return await _plusserContext.Gatherings
            .Include(gathering => gathering.Group)
            .Select(currentEvent => currentEvent.Group!.TelegramId).Distinct()
            .ToListAsync();
    }
}