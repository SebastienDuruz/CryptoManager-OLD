using CryptoManager.Models;
using Newtonsoft.Json;

namespace CryptoManager.Data
{
    public class UserAccountService
    {
        public List<UserAccount> UserAccounts { get; set; }
        private string FilePath { get; set; }

        public UserAccountService()
        {
            this.FilePath = "accounts.json";
            this.UserAccounts = this.ReadUserAccounts();
        }

        public void AddOrUpdateUserAccount(UserAccount newAccount)
        {
            if(this.UserAccounts.Where(x => x.Uuid == newAccount.Uuid).Any())
                this.DeleteUserAccount(newAccount.Uuid);

            this.UserAccounts.Add(newAccount);
            this.WriteUserAccounts();
        }

        public void DeleteUserAccount(string accountUuid)
        {
            this.UserAccounts.Remove(this.UserAccounts.Where(x => x.Uuid == accountUuid).FirstOrDefault());
            this.WriteUserAccounts();
        }

        private List<UserAccount> ReadUserAccounts()
        {
            if(File.Exists(this.FilePath))
                try
                {
                    return JsonConvert.DeserializeObject<List<UserAccount>>(File.ReadAllText(this.FilePath));
                }
                catch(Exception ex)
                {
                    // TODO : Notify the user and ask for action to take with corrupted file
                    this.UserAccounts = new List<UserAccount>();
                    //this.WriteUserAccounts();
                }

            return new List<UserAccount>();
        }

        private void WriteUserAccounts()
        {
            File.WriteAllText(this.FilePath, JsonConvert.SerializeObject(this.UserAccounts.OrderBy(x => x.Coin)));
        }
    }
}
