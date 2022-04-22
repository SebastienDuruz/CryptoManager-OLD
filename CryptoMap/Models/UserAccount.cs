using System.ComponentModel.DataAnnotations;

namespace CryptoMap.Models
{
    public class UserAccount
    {
        public string Uuid { get; } = Guid.NewGuid().ToString();
        [Required, StringLength(40, ErrorMessage = "Name is too long."), MinLength(3, ErrorMessage = "Name is too short.")]
        public string Name { get; set; }
        [Required]
        public string Coin { get; set; }
        [Required]
        public double CoinAmount { get; set; }
    }
}
