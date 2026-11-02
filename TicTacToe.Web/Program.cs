using Microsoft.Extensions.AI;
using OllamaSharp;
using TicTacToe.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IChatClient>(_ => new OllamaApiClient(
    new Uri(builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434"),
    builder.Configuration["Ollama:Model"] ?? "gemma4"));
builder.Services.AddSingleton<GameStore>();

var app = builder.Build();
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllerRoute(name: "default", pattern: "{controller=Game}/{action=Index}/{id?}");
app.Run();
