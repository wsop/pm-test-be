using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CountriesDb"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("*");
    });
});


var app = builder.Build();


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot"))
});

app.MapGet("/", () => Results.Redirect("/index.html"));


app.UseCors();

#region Endpoints

app.MapGet("/api/countries-provinces", async (AppDbContext db) =>
    {
        return Results.Ok(
            await db.Countries
                .Include(c => c.Provinces)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Provinces = c.Provinces.Select(p => new { p.Id, p.Name })
                })
                .ToListAsync());
    })
    .WithOpenApi();

app.MapGet("/api/countries", async (AppDbContext db) =>
    {
        return Results.Ok(await db.Countries
            .Select(c => new { c.Id, c.Name })
            .ToListAsync());
    })
    .WithOpenApi();

app.MapGet("/api/countries/{countryId:int}/provinces", async (int countryId, AppDbContext db) =>
    {
        var provinces = await db.Provinces
            .Where(p => p.CountryId == countryId)
            .Select(p => new { p.Id, p.Name })
            .ToListAsync();

        if (!provinces.Any())
            return Results.NotFound(new { Message = $"Country with Id {countryId} not found or has no provinces." });

        return Results.Ok(provinces);
    })
    .WithOpenApi();


app.MapGet("/api/users", async (AppDbContext dbContext) =>
    {
        var users = await dbContext.Users
            .Select(user => new
            {
                user.Id,
                user.Username,
                user.CountryId,
                user.CityId
            })
            .ToListAsync();

        return Results.Ok(users);
    })
    .WithOpenApi();

#endregion

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Apply seeding
}

app.Run();