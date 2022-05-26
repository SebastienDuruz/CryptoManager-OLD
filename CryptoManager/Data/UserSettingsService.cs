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
                return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(this.FilePath));

            return new UserSettings();
        }

        public void WriteUserSettings()
        {
            File.WriteAllText(this.FilePath, JsonConvert.SerializeObject(this.UserSettings));
        }

        public void WriteUserSettings(UserSettings newSettings)
        {
            File.WriteAllText(this.FilePath, JsonConvert.SerializeObject(newSettings));
            this.UserSettings = newSettings;
        }
    }
}
