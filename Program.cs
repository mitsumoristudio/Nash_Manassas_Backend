using System.Text;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;

// Load ENV file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthorization();

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

var neonAPIKey = Environment.GetEnvironmentVariable("NEON_API_KEY");
// Setting up Neon database
// var connectionString = builder.Configuration.GetConnectionString("NeonConnection");

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



// Alternative from using extension method to add service scope
 //builder.Services.AddScoped<IProjectService, ProjectService>();

var app = builder.Build();
// This order matters for authentication to work
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

app.MapGet("/", () => "Hello World!");

app.Run();

