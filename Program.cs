var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Swagger services (Swashbuckle)
builder.Services.AddSwaggerGen();  // Add this line to register Swagger

builder.Services.AddSingleton<GoogleSheetsService>();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowAllOrigins", policy =>
    //{
    //    policy.AllowAnyOrigin()    // Allow all origins
    //          .AllowAnyMethod()    // Allow all HTTP methods (GET, POST, etc.)
    //          .AllowAnyHeader();   // Allow all headers

    //});
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:8080")  // Add the frontend domain
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443;  // حدد المنفذ الصحيح لـ HTTPS
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
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
