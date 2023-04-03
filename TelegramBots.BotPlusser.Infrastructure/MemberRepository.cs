using Microsoft.EntityFrameworkCore;

using TelegramBots.BotPlusser.Domain.Abstractions;
using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Infrastructure;

public class MemberRepository : IMemberRepository
{
    private readonly PlusserContext _plusserContext;
    private readonly ITelegramBotClient _telegramBotClient;

    public MemberRepository(
        PlusserContext plusserContext,
        ITelegramBotClient telegramBotClient)
    {
        _plusserContext = plusserContext;
        _telegramBotClient = telegramBotClient;
    }

    public async Task<Member?> GetMemberAsync(long telegramId)
    {
        return await _plusserContext.Members
            .FirstOrDefaultAsync(member => member.TelegramId == telegramId);
    }

    public async Task<Member> GetOrCreateMemberAsync(long groupTelegramId, long memberTelegramId)
    {
        var member = await _plusserContext.Members.FirstOrDefaultAsync(member => member.TelegramId == memberTelegramId);
        if (member is null)
        {
            var chatMember = await _telegramBotClient.GetChatMemberAsync(groupTelegramId, memberTelegramId);
            member = new Member(chatMember.User);
        }

        return member;
    }

    public async Task DeleteMembersNotRegisteredInTelegramNotAttendingAnyGatheringsAsync()
    {
        var membersToRemove = await _plusserContext.Members
            .Where(user => !user.TelegramId.HasValue && !user.Attendees.Any())
            .ToListAsync();

        foreach (var memberToRemove in membersToRemove)
        {
            _plusserContext.Remove(memberToRemove);
        }
    }
}