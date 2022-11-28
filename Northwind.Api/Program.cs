using Microsoft.EntityFrameworkCore;
using Northwind.Api.Middlewares;
using Northwind.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NorthwindContext>();
    var migrations = dbContext.Database.GetMigrations().ToHashSet();
    if (dbContext.Database.GetAppliedMigrations().Any(a => !migrations.Contains(a)))
        throw new InvalidOperationException(
            "There is already a migration running on the database that has since been deleted from the project. Delete the database or fix the status of the migrations and then restart the application.");
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
