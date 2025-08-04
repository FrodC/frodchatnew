using ChatVerse.Shared;
using ChatVerse.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, cfg) => cfg.WriteTo.Console());

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=chatverse.db"));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var key = Encoding.UTF8.GetBytes("SuperSecretKey1234!");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

// Minimal API endpoints for auth
app.MapPost("/api/auth/register", async (UserManager<AppUser> userManager, string email, string password) =>
{
    var user = new AppUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded) return Results.BadRequest(result.Errors);
    return Results.Ok();
});

app.MapPost("/api/auth/login", async (UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, string email, string password) =>
{
    var user = await userManager.FindByEmailAsync(email);
    if (user == null) return Results.Unauthorized();
    if (!await userManager.CheckPasswordAsync(user, password)) return Results.Unauthorized();
    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(
        new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Id) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)));
    return Results.Ok(new { token });
});

app.MapHub<ChatHub>("/hub/chat");

app.Run();

// SignalR hub
class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task JoinRoom(Guid roomId, string? roomPassword) => await Task.CompletedTask;
    public async Task LeaveRoom(Guid roomId) => await Task.CompletedTask;
    public async Task SendMessage(Guid roomId, string message) => await Task.CompletedTask;
    public async Task SendWebRtcOffer(Guid roomId, string offerJson) => await Task.CompletedTask;
    public async Task SendWebRtcAnswer(Guid roomId, string answerJson) => await Task.CompletedTask;
    public async Task SendWebRtcCandidate(Guid roomId, string candJson) => await Task.CompletedTask;
}
