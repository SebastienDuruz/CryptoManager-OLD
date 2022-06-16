using ElectronNET.API;
using ElectronNET.API.Entities;

namespace CryptoManager.ElectronApp
{
    public static class ElectronHandler
    {
        private static BrowserWindow AppMainWindow { get; set; }

        public async static void CreateElectronWindow()
        {
            AppMainWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions()
                {
                    Width = 1300,
                    Height = 1000,
                    Resizable = false,
                    Maximizable = false,
                    AutoHideMenuBar = true,
                    Title = "Crypto Manager",
                });

            AppMainWindow.Show();

            // Add events to mainWindow
            AppMainWindow.OnClosed += () => Electron.App.Quit();
        }

        public async static void ReloadMainWindow()
        {
            AppMainWindow.Reload();
            AppMainWindow.FocusOnWebView();
        }
    }
}
