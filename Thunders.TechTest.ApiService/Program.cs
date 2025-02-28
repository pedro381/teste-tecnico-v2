using Thunders.TechTest.ServiceDefaults;
using Thunders.TechTest.ApiService.Validators;
using Thunders.TechTest.OutOfBox.Database;
using Thunders.TechTest.OutOfBox.Queues;
using FluentValidation.AspNetCore;
using Serilog;
using FluentValidation;
using Thunders.TechTest.ApiService;
using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.Application.Messaging;
using Thunders.TechTest.Application.Services;
using Thunders.TechTest.Application.Interfaces;
using Thunders.TechTest.Infrastructure.Data;
using Thunders.TechTest.Infrastructure.Repositories;
using Thunders.TechTest.Infrastructure.Interfaces;
using Thunders.TechTest.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<TollUsageValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

var features = Features.BindFromConfiguration(builder.Configuration);
if (features.UseMessageBroker)
{
    builder.Services.AddBus(builder.Configuration, new SubscriptionBuilder().Add<TollUsage>());
}
if (features.UseEntityFramework)
{
    builder.Services.AddSqlServerDbContext<TollDataContext>(builder.Configuration);
}

builder.Services.AddScoped<IMessageSender, RebusMessageSender>();

builder.Services.AddScoped<ITollDataRepository, TollDataRepository>();
builder.Services.AddScoped<ITollDataService, TollDataService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<TollUsageMessageHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TollDataContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}

app.MapDefaultEndpoints();
app.MapControllers();

app.Run();
