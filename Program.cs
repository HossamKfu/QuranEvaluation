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

// Skip HTTPS redirection in production as Render already handles it
if (!builder.Environment.IsDevelopment())
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
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;  // Make Swagger UI available at the root URL
    });
}

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();  // Only enable this for production

app.UseAuthorization();
app.MapControllers();
app.Run();
