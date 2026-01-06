var builder = WebApplication.CreateBuilder(args);

// Setup services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

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

// Skip HTTPS redirection in development
if (builder.Environment.IsProduction())
{
    // Remove this since Render handles HTTPS
    // builder.Services.AddHttpsRedirection(options => { options.HttpsPort = 443; });
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI only in development
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;  // Make Swagger UI available at the root URL
    });
}

// Use CORS policy and enable HTTPS redirection only if Render doesn't handle HTTPS (this won't be needed in most cases).
app.UseCors("AllowAllOrigins");

app.UseAuthorization();
app.MapControllers();
app.Run();
