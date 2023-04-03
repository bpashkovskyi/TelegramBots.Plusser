using TelegramBots.BotPlusser.Application.Abstractions;
using TelegramBots.BotPlusser.Domain.Abstractions;
using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Application;

public class GatheringService : IGatheringService
{
    private readonly IRollbar _rollbar;
    private readonly IGatheringRepository _gatheringRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ITelegramMessageService _telegramMessageService;

    public GatheringService(
        IRollbar rollbar,
        IGatheringRepository gatheringRepository,
        IMemberRepository memberRepository,
        IGroupRepository groupRepository,
        ITelegramMessageService telegramMessageService)
    {
        _rollbar = rollbar;
        _gatheringRepository = gatheringRepository;
        _memberRepository = memberRepository;
        _groupRepository = groupRepository;
        _telegramMessageService = telegramMessageService;
    }

    public async Task GetNextGatheringPropertyMessage(long creatorTelegramId, string propertyValue)
    {
        var nonDraftGathering = await _gatheringRepository.GetNonDraftGatheringsByCreatorAsync(creatorTelegramId);

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

    public async Task CreateGathering(long creatorTelegramId, long groupTelegramId)
    {
        var gathering = await _gatheringRepository.GetNonDraftGatheringsByCreatorAsync(creatorTelegramId);
        if (gathering != null)
        {
            _rollbar.Warning($"User {creatorTelegramId} tried to create new event in {groupTelegramId}, but has draft event {gathering.Name} in chat {gathering.Group!.TelegramId} not completed");

            await _telegramMessageService.SendNonCompletedGatheringCreationMessageAsync(creatorTelegramId);
            return;
        }

        var creator = await _memberRepository.GetOrCreateMemberAsync(groupTelegramId, creatorTelegramId);
        var group = await _groupRepository.GetOrCreateGroupAsync(groupTelegramId);
        
        gathering = new Gathering(group, creator);

        await _gatheringRepository.AddAsync(gathering);
        await _gatheringRepository.SaveChangesAsync();

        var firstQuestion = gathering.GetPropertyMessage();
        if (firstQuestion != null)
        {
            await _telegramMessageService.SendTextMessageAsync(creatorTelegramId, firstQuestion);
        }
        else
        {
            await _telegramMessageService.SendErrorMessageAsync(creatorTelegramId);
        }
    }

    public async Task DeleteGathering(int gatheringId)
    {
        var gathering = await _gatheringRepository.GetGatheringAsync(gatheringId);

        _gatheringRepository.Delete(gathering);

        await _memberRepository.DeleteMembersNotRegisteredInTelegramNotAttendingAnyGatheringsAsync();

        await _gatheringRepository.SaveChangesAsync();

        if (gathering.PinnedMessageId != null)
        {
            await _telegramMessageService.UpdateGatheringChatPinnedMessageAsync(gathering);
            await _telegramMessageService.SendTextMessageAsync(gathering.Group!.TelegramId, $"Подію \"{gathering.Name}\" видалено");
        }
    }

    public async Task DeleteGroupGathering(long groupTelegramId)
    {
        var group = await _groupRepository.GetGroupAsync(groupTelegramId);
        if (group!.NonDraftGatherings.Count > 1)
        {
            // let user choose which event to delete
            await _telegramMessageService.SendGatheringChooseMessageAsync(group, "deleteevent");
        }
        else
        {
            var gatheringId = group.NonDraftGatherings.First().Id;
            await DeleteGathering(gatheringId);
        }
    }

    public async Task RefreshGroupGroupGatherings(long groupTelegramId)
    {
        var group = await _groupRepository.GetGroupAsync(groupTelegramId);

        foreach (var gathering in group!.NonDraftGatherings)
        {
            await _telegramMessageService.UpdateGatheringChatPinnedMessageAsync(gathering);

            _gatheringRepository.Update(gathering);
            await _gatheringRepository.SaveChangesAsync();
        }
    }

    public async Task BroadcastMessage(string messageText)
    {
        var groupsTelegramIds = await _groupRepository.GetGroupsTelegramIdsAsync();

        foreach (var groupTelegramId in groupsTelegramIds)
        {
            await _telegramMessageService.SendTextMessageAsync(groupTelegramId, messageText);
        }
    }
}