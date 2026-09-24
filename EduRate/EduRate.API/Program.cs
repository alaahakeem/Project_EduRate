using EduRate.API.Middleware;
using EduRate.Application;
using EduRate.Infrastructure;
using EduRate.Infrastructure.BackgroundJobs;
using EduRate.Infrastructure.Identity;
using EduRate.Infrastructure.Persistence;
using EduRate.Infrastructure.Realtime;
using Hangfire;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// ---------- Controllers / Swagger ----------
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------- CORS (allows the frontend dev servers to call the API, including SignalR credentials) ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // Next.js / React / Vite ports
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ---------- Application + Infrastructure (Clean Architecture composition) ----------
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// ---------- HTTP pipeline ----------
// Global exception handling first, so every downstream exception (including from MediatR
// handlers) is converted into a consistent JSON error response.
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

// Hangfire dashboard - handy for confirming the recurring job is registered/running.
// Defaults to local-requests-only; add a proper IDashboardAuthorizationFilter before
// exposing this publicly in production.
app.MapHangfireDashboard("/hangfire");

// The ONE Hangfire recurring job in this project: replaces the original
// SessionReminderService polling loop. Runs every 15 minutes, same as the original interval.
RecurringJob.AddOrUpdate<SessionReminderJob>(
    "session-reminder-job",
    job => job.CheckUpcomingSessionsAsync(),
    "*/15 * * * *");

// ---------- Database seed (migrate + sample data), same as the original inline seeder ----------
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
