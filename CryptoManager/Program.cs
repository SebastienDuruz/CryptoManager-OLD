using CryptoManager.Data;
using CryptoManager.ElectronApp;
using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseElectron(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<CoinGeckoFetcher>();
builder.Services.AddSingleton<UserAccountService>();
builder.Services.AddSingleton<UserSettingsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Open Electron Window
if (HybridSupport.IsElectronActive)
{
    ElectronHandler.CreateElectronWindow();
}

app.Run();

