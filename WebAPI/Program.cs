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
        options.RoutePrefix = "swagger"; // Serve Swagger UI at /docs
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

app.MapGet("/markdown", () =>
{
    var markdown = """
    # 🧪 MVC Application Task: Render Mock Location Data

    ## 🎯 Objective

    Create an ASP.NET MVC application that fetches mock location data from a REST API and displays it in a user-friendly format.

    ## 📡 API Endpoint

    Send a GET request to the following endpoint:

    ```
    GET /locations?page=1&pageSize=10
    ```

    The API returns a paginated list of location items in the following format:

    ```json
    {
      "page": 1,
      "pageSize": 10,
      "totalItems": 100,
      "data": [
        {
          "locationName": "New Jeanne",
          "imageUrl": "https://picsum.photos/640/480/?image=333",
          "timestamp": "2025-08-20T10:19:09.54407-07:00"
        },
        ...
      ]
    }
    ```

    ## 🛠️ Requirements

    - Build an MVC web application using ASP.NET Core.
    - Create a controller that sends a request to the `/locations` endpoint.
    - Deserialize the JSON response into a model.
    - Render the list of locations in a Razor view.
    - Each item should display:
      - 📍 `locationName`
      - 🖼️ `imageUrl` (as an actual image)
      - 🕒 `timestamp` (formatted as a readable date/time)

    ## 🌟 Bonus Points

    - Add pagination controls to navigate between pages.
    - Format timestamps using local time and a friendly format.
    - Make the UI responsive and visually appealing.

    ## 📦 Deliverables

    - Source code of the MVC application.
    - Instructions to run the project locally.
    - A brief explanation of your approach and any design decisions.
    """;

    return Results.Text(markdown, "text/markdown");
})
.WithName("markdown")
.WithMetadata(new ExcludeFromDescriptionAttribute()); ;


app.MapGet("/requirement", () =>
{
    var html = """
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>Markdown Viewer</title>
        <script src="https://cdn.jsdelivr.net/npm/marked/marked.min.js"></script>
        <style>
            body {
                font-family: sans-serif;
                margin: 2rem;
                background-color: #fafafa;
                color: #333;
            }
            #content {
                transition: opacity 0.3s ease-in-out;
            }
            .spinner {
                border: 4px solid #f3f3f3;
                border-top: 4px solid #3498db;
                border-radius: 50%;
                width: 30px;
                height: 30px;
                animation: spin 1s linear infinite;
                margin: 2rem auto;
            }
            @keyframes spin {
                0% { transform: rotate(0deg); }
                100% { transform: rotate(360deg); }
            }
        </style>
    </head>
    <body>
        <main>
            <h1>📄 Markdown Viewer</h1>
            <div class="spinner" id="loader"></div>
            <div id="content" style="opacity: 0;"></div>
        </main>
        <script>
            fetch('/markdown')
                .then(res => {
                    if (!res.ok) throw new Error("Failed to load markdown");
                    return res.text();
                })
                .then(md => {
                    document.getElementById('content').innerHTML = marked.parse(md);
                    document.getElementById('loader').style.display = 'none';
                    document.getElementById('content').style.opacity = 1;
                })
                .catch(err => {
                    document.getElementById('loader').style.display = 'none';
                    document.getElementById('content').innerHTML = "<p style='color:red;'>Error loading markdown: " + err.message + "</p>";
                    document.getElementById('content').style.opacity = 1;
                });
        </script>
    </body>
    </html>
    """;

    return Results.Text(html, "text/html");
})
.WithMetadata(new ExcludeFromDescriptionAttribute()); ;



app.Run();

