using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

using TelegramBots.BotPlusser.Application;
using TelegramBots.BotPlusser.Application.Abstractions;
using TelegramBots.BotPlusser.Domain.Abstractions;
using TelegramBots.BotPlusser.Infrastructure;
using TelegramBots.Shared.Extensions;

namespace TelegramBots.BotPlusser.Presentation;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Presentation
        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddUpdatesMediator();

        // Application
        builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
        builder.Services.AddScoped<IGatheringService, GatheringService>();

        // Persistence
        builder.Services.AddDbContext<PlusserContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("PlusserConnection")!,
            sqlServerDbContextOptionsBuilder =>
                sqlServerDbContextOptionsBuilder.MigrationsHistoryTable("__MigrationsHistory", "plusser")));

        // Infrastructure
        builder.Services.AddScoped<ITelegramMessageService, TelegramMessageService>();

        builder.Services.AddScoped<IGatheringRepository, GatheringRepository>();
        builder.Services.AddScoped<IGroupRepository, GroupRepository>();
        builder.Services.AddScoped<IMemberRepository, MemberRepository>();

        builder.Services.AddRollbar(builder.Configuration);
        builder.Services.AddTelegramClient(builder.Configuration);

        builder.Services.AddHealthChecks(builder.Configuration)
            .AddDbContextCheck<PlusserContext>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());

        app.UseHealthChecks(
            "/healthchecks",
            new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        app.Run();
    }
}