using TelegramBots.BotPlusser.Application.Abstractions;
using TelegramBots.BotPlusser.Domain.Abstractions;
using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Application;

public class SubscriptionService : ISubscriptionService
{
    private readonly IGatheringRepository _gatheringRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ITelegramMessageService _telegramMessageService;

    public SubscriptionService(
        IGatheringRepository gatheringRepository,
        IMemberRepository memberRepository,
        IGroupRepository groupRepository,
        ITelegramMessageService telegramMessageService)
    {
        _gatheringRepository = gatheringRepository;
        _memberRepository = memberRepository;
        _groupRepository = groupRepository;
        _telegramMessageService = telegramMessageService;
    }

    public async Task SubscribeToGathering(int gatheringId, long memberTelegramId)
    {
        var gathering = await _gatheringRepository.GetGatheringAsync(gatheringId);

        var member = await _memberRepository.GetOrCreateMemberAsync(gathering.Group!.TelegramId, memberTelegramId);

        await SubscribeToGathering(gathering, member);
    }

    public async Task SubscribeToGathering(int gatheringId, string memberName)
    {
        var gathering = await _gatheringRepository.GetGatheringAsync(gatheringId);

        var member = new Member(memberName);

        await SubscribeToGathering(gathering, member);
    }

    public async Task SubscribeToGroupGathering(long groupTelegramId, long memberTelegramId)
    {
        var group = await _groupRepository.GetGroupAsync(groupTelegramId);
        if (group!.NonDraftGatherings.Count > 1)
        {
            await _telegramMessageService.SendSubscribeGatheringChooseMessageAsync(memberTelegramId, group);
        }
        else
        {
            var gatheringId = group.NonDraftGatherings.First().Id;
            await SubscribeToGathering(gatheringId, memberTelegramId);
        }
    }


    public async Task SubscribeToGroupGathering(long groupTelegramId, string memberName)
    {
        var group = await _groupRepository.GetGroupAsync(groupTelegramId);
        if (group!.NonDraftGatherings.Count > 1)
        {
            await _telegramMessageService.SendSubscribeGatheringChooseMessage(memberName, group);
        }
        else
        {
            var gatheringId = group.NonDraftGatherings.First().Id;
            await SubscribeToGathering(gatheringId, memberName);
        }
    }

    public async Task UnsubscribeFromGathering(int gatheringId, long memberTelegramId)
    {
        var gathering = await _gatheringRepository.GetGatheringAsync(gatheringId);

        var member = await _memberRepository.GetOrCreateMemberAsync(gathering.Group!.TelegramId, memberTelegramId);

        await UnsubscribeFromGathering(gathering, member);
    }

    public async Task UnsubscribeFromGathering(int gatheringId, string memberName)
    {
        var gathering = await _gatheringRepository.GetGatheringAsync(gatheringId);

        var member = new Member(memberName);

        await UnsubscribeFromGathering(gathering, member);
    }

    public async Task UnsubscribeFromGroupGathering(long groupTelegramId, long memberTelegramId)
    {
        var group = await _groupRepository.GetGroupAsync(groupTelegramId);
        if (group!.NonDraftGatherings.Count > 1)
        {
            await _telegramMessageService.SendUnsubscribeGatheringChooseMessageAsync(memberTelegramId, group);
        }
        else
        {
            var gatheringId = group.NonDraftGatherings.First().Id;
            await UnsubscribeFromGathering(gatheringId, memberTelegramId);
        }
    }

    public async Task UnsubscribeFromGroupGathering(long groupTelegramId, string memberName)
    {
        var group = await _groupRepository.GetGroupAsync(groupTelegramId);
        if (group!.NonDraftGatherings.Count > 1)
        {
            await _telegramMessageService.SendUnsubscribeGatheringChooseMessageAsync(memberName, group);
        }
        else
        {
            var gatheringId = group.NonDraftGatherings.First().Id;
            await UnsubscribeFromGathering(gatheringId, memberName);
        }
    }

    private async Task SubscribeToGathering(Gathering gathering, Member member)
    {
        var subscriptionResult = gathering.SubscribeMember(member);

        await _telegramMessageService.SendTextMessageAsync(gathering.Group!.TelegramId, subscriptionResult.Message);
        await _telegramMessageService.UpdateGatheringChatPinnedMessageAsync(gathering);

        _gatheringRepository.Update(gathering);
        await _gatheringRepository.SaveChangesAsync();
    }

    private async Task UnsubscribeFromGathering(Gathering gathering, Member member)
    {
        var unSubscriptionResult = gathering.UnSubscribeMember(member);

        await _telegramMessageService.SendTextMessageAsync(gathering.Group!.TelegramId, unSubscriptionResult.Message);
        await _telegramMessageService.UpdateGatheringChatPinnedMessageAsync(gathering);

        _gatheringRepository.Update(gathering);
        await _gatheringRepository.SaveChangesAsync();
    }
}