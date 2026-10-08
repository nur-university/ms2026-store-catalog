using Catalog.Application;
using Catalog.Infrastructure;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.EFCore;
using Nur.Store2026.Observability;

var builder = Host.CreateApplicationBuilder(args);

string serviceName = "catalog.worker-service";

builder.UseLogging(serviceName, builder.Configuration);

builder.Services.AddAplication()
                .AddInfrastructure(builder.Configuration, builder.Environment, serviceName);
builder.Services.AddOutboxBackgroundService<DomainEvent>();

var host = builder.Build();
host.Run();
