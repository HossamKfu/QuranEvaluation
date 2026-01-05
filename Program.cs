var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Swagger services (Swashbuckle)
builder.Services.AddSwaggerGen();  // Add this line to register Swagger

builder.Services.AddSingleton<GoogleSheetsService>();
builder.Services.AddHttpClient();

// CORS Policy to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()    // Allow all origins
              .AllowAnyMethod()    // Allow all HTTP methods (GET, POST, etc.)
              .AllowAnyHeader();   // Allow all headers
    });
});

// Add HTTPS Redirection (ensure only if required)
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443;  // Specify the correct HTTPS port
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve Swagger UI (HTML, CSS, JS, etc.)
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // Specify Swagger endpoint
        c.RoutePrefix = string.Empty;  // This makes Swagger UI available at the root URL
    });
}

app.UseCors("AllowAllOrigins");

// Only use HTTPS redirection if not already redirected (prevent infinite redirect loops)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();   // Enable HTTPS redirection in production
}

app.UseAuthorization();

app.MapControllers();

app.Run();
