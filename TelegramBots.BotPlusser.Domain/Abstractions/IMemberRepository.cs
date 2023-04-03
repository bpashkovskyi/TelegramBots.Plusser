using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Domain.Abstractions;

public interface IMemberRepository
{
    Task<Member> GetOrCreateMemberAsync(long groupTelegramId, long memberTelegramId);

    Task DeleteMembersNotRegisteredInTelegramNotAttendingAnyGatheringsAsync();
}