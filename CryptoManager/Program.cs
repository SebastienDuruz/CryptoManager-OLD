using CryptoManager.Data;
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

if (HybridSupport.IsElectronActive)
{
    CreateElectronWindow();
}

app.Run();

async void CreateElectronWindow()
{
    var window = await Electron.WindowManager.CreateWindowAsync(
        new BrowserWindowOptions() {
            Width = 1300,
            Height = 1000,
            Resizable = false,
            Maximizable = false,
            Title = "Crypto Manager"
        }); 

    window.OnClosed += () => Electron.App.Quit();
}


