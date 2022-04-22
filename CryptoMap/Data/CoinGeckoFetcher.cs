using CryptoMap.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CryptoMap.Data
{
    /// <summary>
    /// Fetch the data from CoinGecko API
    /// </summary>
    public class CoinGeckoFetcher
    {
        /// <summary>
        /// HttpClient executing the requests
        /// </summary>
        private HttpClient Client { get; set; }

        /// <summary>
        /// The base URL of the CoinGecko API
        /// </summary>
        private string BaseURL { get; set; }

        public List<CoinGeckoMarket> Coins { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CoinGeckoFetcher()
        {
            this.Client = new HttpClient();
            this.BaseURL = "https://api.coingecko.com/api/v3/";
            this.Coins = GetCoins();
        }

        /// <summary>
        /// Get the top 250 coins from CoinGecko API
        /// </summary>
        /// <returns>Coins list</returns>
        public List<CoinGeckoMarket> GetCoins()
        {
            // Prepare the request
            this.Client = new HttpClient();
            this.Client.BaseAddress = new Uri(this.BaseURL);
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Execute the request
            HttpResponseMessage response = this.Client.GetAsync("coins/markets?vs_currency=usd&order=market_cap_desc&per_page=250&page=1").Result;

            if (response.IsSuccessStatusCode)
            {
                // Parse the response to ActivityModel
                string jsonString = response.Content.ReadAsStringAsync().Result;
                List<CoinGeckoMarket> coins = JsonConvert.DeserializeObject<List<CoinGeckoMarket>>(jsonString);

                return coins;
            }

            return null;
        }

        /// <summary>
        /// Get the full informations about a specific coin
        /// </summary>
        /// <param name="coinId">The coin id to fetch ("bitcoin" for example)</param>
        /// <returns>Coin infos</returns>
        public CoinGeckoFullData GetCoinData(string coinId)
        {
            // Prepare the request
            this.Client = new HttpClient();
            this.Client.BaseAddress = new Uri(this.BaseURL);
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Execute the request
            HttpResponseMessage response = this.Client.GetAsync($"coins/{coinId}?localization=false&tickers=false&market_data=false&community_data=false&developer_data=false&sparkline=false").Result;

            if (response.IsSuccessStatusCode)
            {
                string jsonString = response.Content.ReadAsStringAsync().Result;
                CoinGeckoFullData coinInfos = JsonConvert.DeserializeObject<CoinGeckoFullData>(jsonString);

                // Prepare the request for MARKET DATAS (30 days of prices)
                this.Client = new HttpClient();
                this.Client.BaseAddress = new Uri(this.BaseURL);
                this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Execute the request
                HttpResponseMessage marketResponse = this.Client.GetAsync($"coins/{coinId}/market_chart?vs_currency=usd&days=30&interval=daily").Result;

                if(marketResponse.IsSuccessStatusCode)
                {
                    string jsonMarketString = marketResponse.Content.ReadAsStringAsync().Result;
                    coinInfos.market_datas = JsonConvert.DeserializeObject<MarketData>(jsonMarketString);
                }
                
                return coinInfos;
            }

            return null;
        }
    }
}
