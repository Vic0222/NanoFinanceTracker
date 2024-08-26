using Orleans.Configuration;
using System.Net;
using System;
using Marten;
using Weasel.Core;
using NanoFinanceTracker.Core.Domain.DomainInteraces;
using NanoFinanceTracker.Core.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using NanoFinanceTracker.Core.Application.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);


var isDevelopment = builder.Environment.IsDevelopment();
if (!isDevelopment)
{
    builder.Logging.ClearProviders();
    builder.Logging.AddJsonConsole();
}


// Add orleans
builder.Host.UseOrleans( async siloBuilder =>
{
    string? flyPrivateIP = siloBuilder.Configuration["FLY_PRIVATE_IP"];
    if (!string.IsNullOrEmpty(flyPrivateIP)) //this means we are running in fly.io
    {
        siloBuilder.Configure<EndpointOptions>(options =>
        {
            options.AdvertisedIPAddress = IPAddress.Parse(flyPrivateIP);
        });
    }

    if (isDevelopment)
    {
        siloBuilder.UseLocalhostClustering();
    }
    else
    {
        siloBuilder.UseLocalhostClustering();
    }


    siloBuilder.AddCustomStorageBasedLogConsistencyProviderAsDefault();



});

//Add Marten
// This is the absolute, simplest way to integrate Marten into your
// .NET application with Marten's default configuration
builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    options.Events.StreamIdentity = Marten.Events.StreamIdentity.AsString;
    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
});

// Add services to the container.
builder.Services.AddTransient<IAggregateRepository, AggregateRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<AddExpenseValidator>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    builder.Configuration.Bind("Identity", options);
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
