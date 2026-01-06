var builder = WebApplication.CreateBuilder(args);

// Setup services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<GoogleSheetsService>();
builder.Services.AddHttpClient<GoogleSheetsService>();

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

// Disable HTTPS Redirection in production because Render handles it automatically
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.HttpsPort = 443;  // You can keep this for local dev testing
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

// Remove this from production, Render handles HTTPS for you
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();  // Only enable this for local development or testing
}

app.UseAuthorization();
app.MapControllers();
app.Run();
