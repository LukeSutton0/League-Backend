using League_Backend.Contexts;
using League_Backend.Services.MatchService;
using League_Backend.Services.UserService;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient("RiotApiClient", client =>
{
    string riotApiKey = builder.Configuration["APIKeys:RiotApi"] ?? throw new Exception("API Key cannot be null");
    client.DefaultRequestHeaders.Add("X-Riot-Token", riotApiKey);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MainContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("MainDb")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMatchService, MatchService>();

string? apiKey = builder.Configuration["APIKeys:RiotApi"];

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
