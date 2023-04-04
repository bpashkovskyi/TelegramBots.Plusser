using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBots.BotPlusser.Migrations
{
    public partial class RenameEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "EventsParticipants",
                newName: "Attendees",
                schema: "plusser");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Gatherings",
                schema: "plusser");

            migrationBuilder.RenameTable(
                name: "Chats",
                newName: "Groups",
                schema: "plusser");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Members",
                schema: "plusser");

            migrationBuilder.RenameColumn(
                name: "EventId",
                newName: "GatheringId",
                table: "Attendees",
                schema: "plusser");

            migrationBuilder.RenameColumn(
                name: "UserId",
                newName: "MemberId",
                table: "Attendees",
                schema: "plusser");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Member",
                schema: "plusser");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                newName: "GroupId",
                table: "Gathering",
                schema: "plusser");

            migrationBuilder.RenameColumn(
                name: "CurrentQuestionPropertyName",
                newName: "PropertyToSetName",
                table: "Gathering",
                schema: "plusser");

            migrationBuilder.RenameIndex(
                name: "IX_EventsParticipants_EventId",
                newName: "IX_Attendees_GatheringId",
                table: "Attendees",
                schema: "plusser");

            migrationBuilder.RenameIndex(
                name: "IX_EventsParticipants_UserId",
                newName: "IX_Attendees_MemberId",
                table: "Attendees",
                schema: "plusser");

            migrationBuilder.RenameIndex(
                name: "IX_Events_CreatorId",
                newName: "IX_Gatherings_CreatorId",
                table: "Gatherings",
                schema: "plusser");

            migrationBuilder.RenameIndex(
                name: "IX_Events_ChatId",
                newName: "IX_Gatherings_GroupId",
                table: "Gatherings",
                schema: "plusser");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_TelegramId",
                newName: "IX_Groups_TelegramId",
                table: "Groups",
                schema: "plusser");

            migrationBuilder.RenameIndex(
                name: "IX_Users_TelegramId",
                newName: "IX_Members_TelegramId",
                table: "Members",
                schema: "plusser");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
