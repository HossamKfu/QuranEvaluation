var builder = WebApplication.CreateBuilder(args);

// Setup services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Register GoogleSheetsService and HttpClient
builder.Services.AddSingleton<GoogleSheetsService>();
builder.Services.AddHttpClient<GoogleSheetsService>();  // Ensure GoogleSheetsService can use HttpClient

// CORS Policy to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Bind the application to the correct port depending on the environment (Render handles HTTPS)
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000"; // Default to port 10000 if not set
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");  // Bind to 0.0.0.0 for Render

// Disable HTTPS Redirection in production because Render handles it automatically
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.HttpsPort = 443;  // You can keep this for local dev testing if needed
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;  // Make Swagger UI available at the root URL
    });
}

// Use CORS policy
app.UseCors("AllowAllOrigins");

// HTTPS redirection is not necessary for production since Render handles it
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();  // Only enable this for local development or testing
}

app.UseAuthorization();
app.MapControllers();
app.Run();
