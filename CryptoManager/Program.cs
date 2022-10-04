using CryptoManager.Data;
using CryptoManager.ElectronApp;
using ElectronNET.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<CoinGeckoFetcher>();
builder.Services.AddSingleton<UserAccountService>();
builder.Services.AddSingleton<UserSettingsService>();

builder.WebHost.UseElectron(args);

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
    ElectronHandler.CreateElectronWindow();

app.Run();
