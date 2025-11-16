using System.Text;
using System.Text.Json;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics;
using Nash_Manassas.utils;
using Project_Manassas.Database;
using Project_Manassas.Service;
using ModelContextProtocol.Server;
using Nash_Manassas.Hub;


// Load ENV file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Swagger
// -------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

// -------------------------
// SignalR implementation
// -------------------------
builder.Services.AddSignalR()
    .AddJsonProtocol(opts =>
    {
        opts.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// -------------------------
// JWT authentication
// -------------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer(option =>
    {
        option.RequireHttpsMetadata = false;
        option.SaveToken = true;
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings?.Issuer,
        
            ValidateAudience = true,
            ValidAudience = jwtSettings?.Audience,
        
            ValidateLifetime = true,
        
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SecretKey))
            // Tells ASP.NET Core ow to validate incoming JWT tokens
        
        };
    });


var authDomainKey = Environment.GetEnvironmentVariable("AUTH0_DOMAIN_KEY");
var authClientId = Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID");
var authClientSecret = Environment.GetEnvironmentVariable("AUTH0_CLIENT_SECRET");

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    if (authDomainKey != null) options.Domain = authDomainKey;
    if (authClientId != null) options.ClientId = authClientId;
    if (authClientSecret != null) options.ClientSecret = authClientSecret;
});
builder.Services.AddControllersWithViews();

// Setting up Neon database
var neonAPIKey = Environment.GetEnvironmentVariable("NEON_API_KEY");

builder.Services.AddDbContext<ProjectContext>(options =>
{
    options.UseNpgsql(neonAPIKey);
});

// Add CORS services to container for REACT to call the API on frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy",
        policy => policy.WithOrigins("http://localhost:3000") // URL for React App
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});


// -------------------------
// Add MCP service from MCP server
// -------------------------
// Added dotnet add package ModelContextProtocol --version 0.4.0-preview.3
builder.Services.AddHttpClient<McpBridgeService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/"); // MCP server URL
});

// Adding McpConnection
builder.Services.AddScoped<IMcpConnectionService, McpConnectService>();

// Add McpChatService
builder.Services.AddSingleton<McpChatService>();

// Adding Controller for MVC pattern
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddApplicationServices();



// Alternative from using extension method to add service scope
 //builder.Services.AddScoped<IProjectService, ProjectService>();

var app = builder.Build();
// This order matters for authentication to work

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerPathFeature>();
            var ex = feature?.Error;
            Console.Error.WriteLine(ex?.ToString());
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                Message = "Internal server error",
                Error = ex?.Message,
                StackTrace = ex?.StackTrace
            });
        });
    });
}
else
{
    app.UseDeveloperExceptionPage();
}

app.MapHub<ProjectHub>("/projectHub");

app.UseRouting();

app.UseCors("DevCorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapGet("/health", () => "OK");
app.MapGet("/", () => "Hello User");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.Run();

