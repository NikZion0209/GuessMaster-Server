using GuessMaster.Data.Data;
using GuessMaster.Repository;
using GuessMaster.Repository.Interface;
using GuessMaster.Repository.Repository;
using GuessMaster.Service;
using GuessMaster.Service.Event_Handlers;
using GuessMaster.Service.Interface;
using GuessMaster.Service.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using Amazon.SecretsManager;
using Amazon.Runtime;
using Amazon.Extensions.NETCore.Setup;


var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Services.AddAWSService<IAmazonSecretsManager>();
builder.Services.AddSingleton<JwtKeyProvider>(sp =>
{
    var secretsManager = sp.GetRequiredService<IAmazonSecretsManager>();
    return new JwtKeyProvider(secretsManager, "prod/jwt/signingkey");
});

if (builder.Environment.IsProduction())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5000); // Production: 0.0.0.0:5000
    }); 
}
else
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5000); // HTTP
        options.ListenAnyIP(5001, listenOptions =>
        {
            listenOptions.UseHttps("Certs/localhost+4.pfx", "123456");
        });
    });
}

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
builder.Services.AddSingleton<IDoodleChampRepository, DoodleChampRepository>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IDoodleChamp, DoodleChamp>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IGameTimer, GameTimer>();
builder.Services.AddSingleton<IEventHandler, DoodleChampEventHandler>();
builder.Services.AddSingleton<IJWTHelper, JWTHelper>();

builder.Services.AddSignalR();

builder.Services.AddMemoryCache();

builder.Services.AddHostedService<EventHostedService>();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        // Allow requests from the specified origin
        policy.WithOrigins(
            builder.Configuration["ClientUrl"],
            builder.Configuration["ClientUrlWWW"]
            )
              .AllowAnyHeader()
              .AllowCredentials()
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


string? jwtKey = null;
if (builder.Environment.IsProduction())
{
    using var tempServiceProvider = builder.Services.BuildServiceProvider();
    var jwtKeyProvider = tempServiceProvider.GetRequiredService<JwtKeyProvider>();
    jwtKey = jwtKeyProvider.GetJwtKeyAsync().GetAwaiter().GetResult();

    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new Exception("JWT Key not found in Secrets Manager.");
    }
}
else
{
    // For development generate random key
    jwtKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}

builder.Configuration["Jwt:Key"] = jwtKey;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware pipeline
app.UseSession();  // Enable session middleware
// Enable WebSocket support
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
};
app.UseWebSockets(webSocketOptions);
app.MapHub<ChatHub>("/chatHub");
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins"); // Enable CORS
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtRefreshMiddleware>();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();
