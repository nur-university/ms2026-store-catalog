using Catalog.Application;
using Catalog.Infrastructure;
using Catalog.WebApi;
using Catalog.WebApi.Extensions;
using Nur.Store2026.Observability;

var builder = WebApplication.CreateBuilder(args);

string serviceName = "catalog.api";

builder.Host.UseLogging(serviceName, builder.Configuration);

// Add services to the container.
builder.Services
    .AddAplication()
    .AddInfrastructure(builder.Configuration, builder.Environment, serviceName)
    .AddPresentation(builder.Configuration, builder.Environment);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Must run before UseSwaggerWithUi(): UseSwagger() short-circuits matching
    // requests (e.g. /swagger/v1/swagger.json) without reaching middleware
    // registered later, so CORS has to be in front of it to actually apply.
    app.UseCors("AllowGatewaySwagger");
    app.UseSwaggerWithUi();
    app.ApplyMigrations();
}

app.UseRouting();

app.UseHealthChecks();

app.UseRequestCorrelationId();

app.UseRequestContextLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
