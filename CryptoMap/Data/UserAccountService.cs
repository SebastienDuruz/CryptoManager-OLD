using CryptoMap.Models;

namespace CryptoMap.Data
{
    public class UserAccountService
    {
        public List<UserAccount> UserAccounts { get; set; }

        public UserAccountService()
        {
            this.UserAccounts = new List<UserAccount>() { new UserAccount() { Name = "BTC Main", Coin = "BTC", CoinAmount = 0.45} };
        }

        public void AddOrUpdateUserAccount(UserAccount newAccount)
        {
            if(this.UserAccounts.Where(x => x.Uuid == newAccount.Uuid).Any())
                this.DeleteUserAccount(newAccount.Uuid);

            this.UserAccounts.Add(newAccount);
        }

        public void DeleteUserAccount(string accountUuid)
        {
            this.UserAccounts.Remove(this.UserAccounts.Where(x => x.Uuid == accountUuid).FirstOrDefault());
        }
    }
}
