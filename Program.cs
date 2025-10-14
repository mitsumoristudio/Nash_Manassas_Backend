using System.Text;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics;
using Nash_Manassas.utils;
using Project_Manassas.Database;

// Load ENV file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

// JWT settings
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

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
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

