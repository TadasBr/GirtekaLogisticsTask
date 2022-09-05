using ElectricityDataAPI.Data;
using ElectricityDataAPI.Helper;
using ElectricityDataAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog(logger);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ElectricityDbContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

var app = builder.Build();

await SeedDatabase();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

//Seeds database if database is empty
async Task SeedDatabase()
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetService<ElectricityDbContext>()
                  ?? throw new Exception($"{nameof(ElectricityDbContext)} doesn't exist in services!");

    if (await context.ElectricityReports.AnyAsync()) return;

    var estateCounter = 0;
    var (totalCount, filteredCount) = (0, 0);

    var estates =
        ElectricitySeedingData.ClassifyData(await ElectricitySeedingData.GetHrefs(), CancellationToken.None)
            .Where(report =>
            {
                totalCount++;
                var condition = report.RealEstate.HouseType == HouseType.House && report.ConsumedElectricity is < 1f;
                if (condition) filteredCount++;
                return condition;
            })
            .GroupBy(report => report.RealEstate)
            .SelectAwait(async grouping =>
            {
                grouping.Key.ElectricityReports = await grouping.ToListAsync();
                return grouping.Key;
            });

    await foreach (var estate in estates)
    {
        Console.WriteLine(++estateCounter);
        await context.RealEstates.AddAsync(estate);
    }

    logger.Information("Starting seeding database");

    await context.SaveChangesAsync();

    logger.Information("Finished seeding database");

    //Writes to file the difference between total and filtered Data
    File.WriteAllText("Difference.txt", "The difference between total and filtered data: " + (totalCount - filteredCount));

}
