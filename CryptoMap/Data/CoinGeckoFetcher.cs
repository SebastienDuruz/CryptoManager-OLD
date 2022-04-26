using CryptoMap.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CryptoMap.Data
{
    public class CoinGeckoFetcher
    {
        private HttpClient Client { get; set; }
        private string BaseURL { get; set; }
        public List<CoinGeckoMarket> Coins { get; set; }
        
        public CoinGeckoFetcher()
        {
            this.Client = new HttpClient();
            this.BaseURL = "https://api.coingecko.com/api/v3/";
            this.Coins = GetCoins();
        }

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
