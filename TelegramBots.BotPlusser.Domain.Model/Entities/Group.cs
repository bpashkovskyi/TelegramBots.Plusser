using Telegram.Bot.Types;

namespace TelegramBots.BotPlusser.Domain.Model.Entities;

public class Group : BaseEntity
{
    private readonly List<Gathering> _gatherings = new();

    public Group(Chat telegramChat)
    {
        TelegramId = telegramChat.Id;
        Name = telegramChat.Title!;
    }

    private Group()
    {
    }

    public long TelegramId { get; }

    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<Gathering> Gatherings => _gatherings;

    public IReadOnlyCollection<Gathering> NonDraftGatherings =>
        Gatherings.Where(gathering => !gathering.IsDraft).ToList();
}