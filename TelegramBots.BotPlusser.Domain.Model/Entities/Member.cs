﻿using Telegram.Bot.Types;

namespace TelegramBots.BotPlusser.Domain.Model.Entities;

public class Member : BaseEntity
{
    private readonly List<Attendee> _attendees = new();

    public Member(User user)
        : this()
    {
        FirstName = user.FirstName;
        Name = user.Username;
        TelegramId = user.Id;
    }

    public Member(string name)
    {
        Name = name;
    }

    private Member()
    {
    }

    public long? TelegramId { get; }

    public string? FirstName { get; private set; }

    public string? Name { get; private set; }

    public string DisplayName => TelegramId.HasValue ? $"{FirstName!}" : Name!;

    public IReadOnlyCollection<Attendee> Attendees => _attendees;

    public bool Equals(Member member)
    {
        if (Id != 0 && Id == member.Id)
        {
            return true;
        }

        if (TelegramId.HasValue && TelegramId == member.TelegramId)
        {
            return true;
        }

        return !TelegramId.HasValue && Name!.Equals(member.Name, StringComparison.Ordinal);
    }
}