using System.Text;
using Backend_AIHost.Hubs;
using Backend_AIHost.Services;
using Backend_AIHost.Services.Docker.Interfaces;
using Backend_AIHost.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Backend_AIHost.Services.Docker;
using Backend_AIHost.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["CONNECTION_STRING"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection");



// Konfiguracja usług
builder.Services.ConfigureDatabase(connectionString)
       .ConfigureIdentity();

Console.WriteLine(builder.Configuration["JWT_KEY"] ?? "TwojSuperSekretnyKluczJWT");
Console.WriteLine($"Issuer: {builder.Configuration["Jwt:Issuer"]}");

// JWT
var key = builder.Configuration["JWT_KEY"] ?? "Jwt:Key";
var issuer = builder.Configuration["Jwt:Issuer"];
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
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
    };
    options.MapInboundClaims = false;
});

// SignalR
builder.Services.AddSignalR();

// Serwisy
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVpsService, VpsService>();
builder.Services.AddScoped<IAIModelService, AIModelService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserImageService, UserImageService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IAdminModelService, AdminModelService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IContainerService, ContainerService>();
builder.Services.AddScoped<IDockerService, DockerService>();

builder.Services.AddHttpClient<ApiChecker>();
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

app.UseRouting();

app.UseCors(policy =>
{
    policy.WithOrigins("https://lk-soft.info", "http://localhost:5173")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
});

// Middleware
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// SignalR
app.MapHub<DeploymentHub>("/hubs/deployment");
app.MapHub<LogHub>("/hubs/log");

// Seedowanie danych
await app.Services.SeedDataAsync();

// OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Kontrolery
app.MapControllers();

app.Run();
