using TelegramBots.BotPlusser.Domain.Model.Enums;

namespace TelegramBots.BotPlusser.Domain.Model.Entities;

public class Attendee : BaseEntity
{
    public Attendee(Member member)
    {
        Member = member;
        Status = AttendanceStatus.None;
    }

    private Attendee()
    {
    }


    public Gathering? Gathering { get; private set; }

    public int GatheringId { get; private set; }

    public Member? Member { get; }

    public int MemberId { get; private set; }

    public AttendanceStatus Status { get; private set; }

    public SubscriptionStatus Subscribe()
    {
        if (Status == AttendanceStatus.Going)
        {
            return SubscriptionStatus.AlreadySubscribed;
        }

        Status = AttendanceStatus.Going;
        return SubscriptionStatus.SubscriptionConfirmed;
    }

    public UnSubscriptionStatus UnSubscribe()
    {
        if (Status == AttendanceStatus.NotGoing)
        {
            return UnSubscriptionStatus.AlreadyUnsubscribed;
        }

        Status = AttendanceStatus.NotGoing;

        return UnSubscriptionStatus.UnSubscriptionConfirmed;
    }
}