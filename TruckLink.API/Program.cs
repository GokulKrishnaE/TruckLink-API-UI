using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TruckLink.Core.Interfaces;
using TruckLink.Infrastructure.Data;
using TruckLink.Infrastructure.Repositories;
using TruckLink.Logic.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration setup
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Read allowed origins from config
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

// Log origins to console for debugging
Console.WriteLine("Allowed origins:");
if (allowedOrigins != null)
{
    foreach (var origin in allowedOrigins)
    {
        Console.WriteLine(origin);
    }
}
else
{
    Console.WriteLine("No allowed origins configured.");
}

// Configure CORS with origins and AllowCredentials
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // Important if frontend sends credentials
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var env = builder.Environment;

// Load appsettings.json + env-specific overrides
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Register DbContext with single key: "DefaultConnection"
if (env.IsDevelopment())
{
    builder.Services.AddDbContext<TruckLinkDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else if (env.IsProduction())
{
    builder.Services.AddDbContext<TruckLinkDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

//builder.Services.AddDbContext<TruckLinkDbContext>(options =>
//    options.UseSqlServer(connectionString));

//builder.Services.AddDbContext<TruckLinkDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        new MySqlServerVersion(new Version(8, 2, 0))
//    )
//);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper setup
builder.Services.AddSingleton(provider =>
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.ShouldMapMethod = _ => false;
        cfg.AddProfile<MappingProfile>();
    });

    return config.CreateMapper();
});

// Dependency injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Authentication with JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Set port for Render environment or fallback
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();


//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<TruckLinkDbContext>();
//    db.Database.Migrate();
//}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<TruckLinkDbContext>();
            db.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying migrations.");
        }
    }
}

// Optional: comment out if Render does not support HTTPS properly
// app.UseHttpsRedirection();

app.UseRouting();

// IMPORTANT: UseCors between UseRouting and UseAuthentication
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
