namespace CryptoManager.Models
{
    public class UserSettings
    {
        public int CoinAmount { get; set; } = 200;
        public int CoinPerPage { get; set; } = 50;
        public bool CoinDistributionChart { get; set; } = true;
        public bool CategoryDistributionChart { get; set; } = true;
        public string Theme { get; set; } = "Cyborg";

    }
}
