using TelegramBots.BotPlusser.Domain.Model.Entities;

namespace TelegramBots.BotPlusser.Persistence.Base;

public interface IMemberRepository
{
    Task<Member> GetOrCreateMemberAsync(long groupTelegramId, long memberTelegramId);

    Task DeleteMembersNotRegisteredInTelegramNotAttendingAnyGatheringsAsync();
}