using TelegramBots.BotPlusser.Application.Base;
using TelegramBots.BotPlusser.Domain.Model.Entities;
using TelegramBots.BotPlusser.Infrastructure.Base;
using TelegramBots.BotPlusser.Persistence.Base;

namespace TelegramBots.BotPlusser.Application;

public class GatheringService : IGatheringService
{
    private readonly IGatheringRepository _gatheringRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ITelegramMessageService _telegramMessageService;

    public GatheringService(
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

    public async Task GetNextGatheringPropertyMessageAsync(long creatorTelegramId, string propertyValue)
    {
        var nonDraftGathering = await _gatheringRepository.GetNonDraftGatheringCreatedByAsync(creatorTelegramId);

        nonDraftGathering!.SetPropertyValue(propertyValue);

        var nextPropertyMessage = nonDraftGathering.GetPropertyMessage();
        if (nextPropertyMessage == null)
        {
            await _telegramMessageService.SendGatheringCreatedMessageAsync(nonDraftGathering);
            await _telegramMessageService.UpdateGatheringChatPinnedMessageAsync(nonDraftGathering);
            nonDraftGathering.MarkAsNonDraft();
        }
        else
        {
            await _telegramMessageService.SendTextMessageAsync(creatorTelegramId, nextPropertyMessage);
        }

        await _gatheringRepository.SaveChangesAsync();
    }

    public async Task CreateGatheringAsync(long creatorTelegramId, long groupTelegramId)
    {
        var gathering = await _gatheringRepository.GetNonDraftGatheringCreatedByAsync(creatorTelegramId);
        if (gathering != null)
        {
            await _telegramMessageService.SendNonCompletedGatheringCreationMessageAsync(creatorTelegramId);
            return;
        }

        var creator = await _memberRepository.GetOrCreateMemberAsync(groupTelegramId, creatorTelegramId);
        var group = await _groupRepository.GetOrCreateGroupAsync(groupTelegramId);
        
        gathering = new Gathering(group, creator);

        await _gatheringRepository.AddAsync(gathering);
        await _gatheringRepository.SaveChangesAsync();

        await SendFirstPropertyMessageAsync(gathering, creatorTelegramId);
    }

    public async Task DeleteGatheringAsync(int gatheringId)
    {
        var gathering = await _gatheringRepository.GetGatheringAsync(gatheringId);

        _gatheringRepository.Delete(gathering);

        await _memberRepository.DeleteMembersNotRegisteredInTelegramNotAttendingAnyGatheringsAsync();

        await _gatheringRepository.SaveChangesAsync();

        if (gathering.PinnedMessageId != null)
        {
            await _telegramMessageService.UnpinChatMessageAsync(gathering);
            await _telegramMessageService.SendTextMessageAsync(gathering.Group!.TelegramId, $"Подію \"{gathering.Name}\" видалено");
        }
    }

    public async Task DeleteGroupGatheringAsync(long groupTelegramId)
    {
        var group = await _groupRepository.GetGroupAsync(groupTelegramId);
        if (group!.NonDraftGatherings.Count > 1)
        {
            // let user choose which event to delete
            await _telegramMessageService.SendGatheringChooseMessageAsync(group, "deletegathering");
        }
        else
        {
            var gatheringId = group.NonDraftGatherings.First().Id;
            await DeleteGatheringAsync(gatheringId);
        }
    }

    public async Task RefreshGroupGroupGatheringsAsync(long groupTelegramId)
    {
        var group = await _groupRepository.GetGroupAsync(groupTelegramId);

        foreach (var gathering in group!.NonDraftGatherings)
        {
            await _telegramMessageService.UpdateGatheringChatPinnedMessageAsync(gathering);

            _gatheringRepository.Update(gathering);
            await _gatheringRepository.SaveChangesAsync();
        }
    }

    public async Task BroadcastMessageAsync(string messageText)
    {
        var groupsTelegramIds = await _groupRepository.GetGroupsTelegramIdsAsync();

        foreach (var groupTelegramId in groupsTelegramIds)
        {
            await _telegramMessageService.SendTextMessageAsync(groupTelegramId, messageText);
        }
    }

    private async Task SendFirstPropertyMessageAsync(Gathering gathering, long creatorTelegramId)
    {
        var firstPropertyMessage = gathering.GetPropertyMessage();
        if (firstPropertyMessage != null)
        {
            await _telegramMessageService.SendTextMessageAsync(creatorTelegramId, firstPropertyMessage);
        }
        else
        {
            await _telegramMessageService.SendErrorMessageAsync(creatorTelegramId);
        }
    }
}