using CryptoManager.Models;
using Newtonsoft.Json;

namespace CryptoManager.Data
{
    public class UserSettingsService
    {
        private string FilePath { get; set; }
        public UserSettings UserSettings { get; set; }

        public UserSettingsService()
        {
            this.FilePath = "settings.json";
            this.UserSettings = this.ReadUserSettings();
        }

        public UserSettings ReadUserSettings()
        {
            if(File.Exists(this.FilePath))
            {
                try
                {
                    return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(this.FilePath));
                }
                catch(Exception ex)
                {
                    // TODO : ask the user for action to take with corrupted file
                }
            }

            return new UserSettings();
        }

        public void WriteUserSettings(UserSettings newSettings)
        {
            File.WriteAllText(this.FilePath, JsonConvert.SerializeObject(newSettings));
            this.UserSettings = newSettings;
        }

        public async Task ReloadSettings()
        {
            if (File.Exists(this.FilePath))
                this.UserSettings = ReadUserSettings();
        }
    }
}
