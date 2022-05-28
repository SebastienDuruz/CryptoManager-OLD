namespace CryptoManager.Models
{
    public class UserSettings
    {
        public int CoinAmount { get; set; } = 200;
        public int CoinPerPage { get; set; } = 50;
        public string Theme { get; set; } = "Cyborg";
    }
}
