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
    builder.Services.AddHttpsRedirection(options =>
    {
        options.HttpsPort = 443;  // Specify HTTPS port for production environment
    });
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

// Use CORS policy and redirect HTTP to HTTPS
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();  // Only enables HTTPS redirection in production

app.UseAuthorization();
app.MapControllers();
app.Run();
