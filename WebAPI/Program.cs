using Bogus;
using Microsoft.OpenApi.Models;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mock Location API",
        Version = "v1",
        Description = "A minimal API that returns mock location data using Bogus"
    });
});

var app = builder.Build();

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mock Location API v1");
        options.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();

// Endpoint with pagination
app.MapGet("/locations", (int page = 1, int pageSize = 10) =>
{
    var faker = new Faker<LocationInfo>()
        .RuleFor(l => l.LocationName, f => f.Address.City())
        .RuleFor(l => l.ImageUrl, f => f.Image.PicsumUrl())
        .RuleFor(l => l.Timestamp, f => f.Date.Recent());

    var totalItems = 100;
    var items = faker.Generate(totalItems)
                     .Skip((page - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();

    return Results.Ok(new
    {
        Page = page,
        PageSize = pageSize,
        TotalItems = totalItems,
        Data = items
    });
})
.WithName("GetLocations")
.WithOpenApi(); // Adds this endpoint to Swagger

app.Run();

