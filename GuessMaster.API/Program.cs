using GuessMaster.API.Middleware;
using GuessMaster.Data.Data;
using GuessMaster.Repository;
using GuessMaster.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Swagger configuration for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database connection configuration
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Dependency Injection for repository and service managers
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        // Allow requests from the specified origin
        policy.WithOrigins("http://127.0.0.1:5500")  // Modify as per your frontend origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Session configuration with distributed memory cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);  // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;  // Essential cookie to handle session
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.MapPost("/api/authenticate", async context =>
//{
//    var apiKey = context.Request.Headers["ApiKey"].ToString();

//    if (apiKey == "sqYELam2PQ5pBLnJQonEynea3W73IRB8sI8vg77qEQI=")
//    {
//        var token = Guid.NewGuid().ToString(); // Example token
//        context.Response.ContentType = "application/json";
//        await context.Response.WriteAsync($"{{\"token\":\"{token}\"}}");
//    }
//    else
//    {
//        context.Response.StatusCode = 403;
//        await context.Response.WriteAsync("Invalid API Key");
//    }
//});

// Middleware pipeline
app.UseSession();  // Enable session middleware
app.UseWebSockets();

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins"); // Enable CORS
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<ApiKeyMiddleware>();  // If using custom API key validation

app.MapControllers();

app.Run();
