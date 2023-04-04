using System.ComponentModel;
using System.Reflection;
using System.Text;

using TelegramBots.BotPlusser.Domain.Attributes;
using TelegramBots.BotPlusser.Domain.Enums;
using TelegramBots.BotPlusser.Domain.ValueObjects;

namespace TelegramBots.BotPlusser.Domain.Entities;

public class Gathering : BaseEntity
{
    private readonly List<Attendee> _attendees = new();

    public Gathering(Group group, Member creator)
    {
        Group = group;
        Creator = creator;
        PropertyToSetName = "Name";
        IsDraft = true;
    }

    private Gathering()
    {
    }

    public bool IsDraft { get; private set; }

    [Message("Вкажіть назву події")]
    public string? Name { get; private set; }

    [Message("Вкажіть максимальну кількість учасників події")]
    public int? MaxParticipantsCount { get; private set; }

    [Message("Додайте опис події")]
    public string? Description { get; private set; }

    public int GroupId { get; private set; }

    public Group? Group { get; }

    public int CreatorId { get; private set; }

    public Member? Creator { get; }

    public int? PinnedMessageId { get; set; }

    public IReadOnlyCollection<Attendee> Attendees => _attendees;

    public string? PropertyToSetName { get; private set; }

    private IEnumerable<Member> GoingMembers => Attendees
        .Where(attendee => attendee is { Status: AttendanceStatus.Going, Member: { } })
        .Select(attendee => attendee.Member!);

    private IEnumerable<Member> NotGoingMembers => Attendees
        .Where(attendee => attendee is { Status: AttendanceStatus.NotGoing, Member: { } })
        .Select(attendee => attendee.Member!);

    public void MarkAsNonDraft()
    {
        IsDraft = false;
    }

    public SubscriptionResult SubscribeMember(Member member)
    {
        var subscriptionStatus = SubscribeGatheringMember(member);

        var subscriptionResult = new SubscriptionResult(subscriptionStatus, Name!);

        return subscriptionResult;
    }

    public UnSubscriptionResult UnSubscribeMember(Member member)
    {
        var unSubscriptionStatus = UnSubscribeGatheringMember(member);

        return new UnSubscriptionResult(unSubscriptionStatus, Name!);
    }

    private SubscriptionStatus SubscribeGatheringMember(Member member)
    {
        if (GoingMembers.Count() == MaxParticipantsCount)
        {
            return SubscriptionStatus.NoPlace;
        }

        var attendee = GetOrCreateAttendee(member);

        return attendee.Subscribe();
    }

    private UnSubscriptionStatus UnSubscribeGatheringMember(Member member)
    {
        var attendee = GetOrCreateAttendee(member);

        return attendee.UnSubscribe();
    }

    public string GetAttendanceInformation()
    {
        var eventAttendanceStringBuilder = new StringBuilder($"{Name}. {Description} ");

        if (GoingMembers.Any())
        {
            eventAttendanceStringBuilder.Append(CultureInfo.InvariantCulture,
                $"Йдуть: {GoingMembers.Count()}/{MaxParticipantsCount} ({ConvertMembersListToString(GoingMembers.ToList())}). ");
        }

        if (NotGoingMembers.Any())
        {
            eventAttendanceStringBuilder.Append(CultureInfo.InvariantCulture,
                $"Не йдуть: {NotGoingMembers.Count()} ({ConvertMembersListToString(NotGoingMembers.ToList())}).");
        }

        return eventAttendanceStringBuilder.ToString();

        static string ConvertMembersListToString(List<Member> members)
        {
            return string.Join(", ", members.Select(member => member.DisplayName));
        }
    }

    public string? GetPropertyMessage()
    {
        if (PropertyToSetName == null)
        {
            return null;
        }

        var property = GetType().GetProperty(PropertyToSetName);
        if (property == null)
        {
            return null;
        }

        return GetMessageAttribute(property)?.Text;
    }

    public void SetPropertyValue(string propertyValue)
    {
        if (PropertyToSetName == null)
        {
            return;
        }

        var propertyToSet = GetType().GetProperty(PropertyToSetName);
        if (propertyToSet == null)
        {
            return;
        }

        SetPropertyValue(propertyValue, propertyToSet);

        MoveToNextProperty();
    }

    private void SetPropertyValue(string text, PropertyInfo propertyInfo)
    {
        var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
        var value = converter.ConvertFromString(null, CultureInfo.InvariantCulture, text);

        propertyInfo.SetValue(this, value);
    }

    private void MoveToNextProperty()
    {
        var allProperties = GetType().GetProperties()
            .Where(propertyInfo => GetMessageAttribute(propertyInfo) != null)
            .OrderBy(propertyInfo => GetMessageAttribute(propertyInfo)!.Order)
            .ToArray();

        var currentPropertyPosition = Array.IndexOf(allProperties,
            allProperties.First(propertyInfo => propertyInfo.Name == PropertyToSetName));

        PropertyToSetName = currentPropertyPosition == allProperties.Length - 1
            ? null
            : allProperties[currentPropertyPosition + 1].Name;
    }

    private MessageAttribute? GetMessageAttribute(PropertyInfo property)
    {
        return property.GetCustomAttribute(typeof(MessageAttribute), false) as MessageAttribute;
    }

    private Attendee GetOrCreateAttendee(Member member)
    {
        var attendee = Attendees.FirstOrDefault(a => a.Member != null && a.Member.Equals(member));
        if (attendee == null)
        {
            attendee = new Attendee(member);
            _attendees.Add(attendee);
        }

        return attendee;
    }
}