using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Services;
using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

EnvironmentDetermination environmentDetermination = new EnvironmentDetermination();
environmentDetermination.IsDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddSingleton<EnvironmentDetermination>(environmentDetermination);

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddDbContext<ATAccountContext>();

builder.Services.AddHttpClient<DriverNodeService>();

builder.Services.AddAuthentication(options =>
{
    //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
//.AddCookie()
// .AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
// {
//     googleOptions.ClientId = "1063124829350-i4c5l73tlci3075l7fvsjmt5pamvn3i4.apps.googleusercontent.com";
//     googleOptions.ClientSecret = "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";
//     googleOptions.ReturnUrlParameter = "https://localhost:44314/api/Login/GoogleResponse";
// });




builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

ConnectionMultiplexer redis = null;

if(builder.Environment.IsDevelopment())
{
     redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisDevCon"));
}
else
{
    redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisProdCon"));
}

builder.Services.AddSingleton<IConnectionMultiplexer>(redis);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddAuthorization();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllDev",
    policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
    options.AddPolicy(name: "dene", policy =>
    {
        policy.WithOrigins("https://anttaxi.mobilulasim.com").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});
builder.Services.AddHealthChecks()
 .AddCheck("self", () => HealthCheckResult.Healthy());
//builder.Services.AddAuthentication()
//    .AddGoogle(googleOptions => {
//        googleOptions.ClientId = "104743001505-4db8mq6lki3ep6pcfl4br0a79l3tlhe4.apps.googleusercontent.com";
//        googleOptions.ClientSecret = "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";
//        googleOptions.ReturnUrlParameter = "https://localhost:44314/api/Login/GetTest";
//    });
//.AddMicrosoftAccount(microsoftOptions =>
//{
//    microsoftOptions.ClientId = "";
//    microsoftOptions.ClientSecret = "";
//});
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)

//        .AddEntityFrameworkStores<ATAccountContext>();

//.AddGoogle(googleOptions => { ... })
//.AddTwitter(twitterOptions => { ... })
//.AddFacebook(facebookOptions => { ... });
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.AzureApp()
    .CreateLogger();

Host.CreateDefaultBuilder(args)
    .UseSerilog();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAllDev");
app.MapControllers();
//app.UseSession();
app.Run();
//Pipeline Trigger