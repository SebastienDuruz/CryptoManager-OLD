using CryptoManager.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CryptoManager.Data
{
    public class CoinGeckoFetcher
    {
        private HttpClient Client { get; set; }
        private string BaseURL { get; set; }
        private int PageCounter { get; set; }
        private int CoinGeckoPageSize { get; set; } = 100;
        public List<CoinGeckoMarket> Coins { get; set; }
        public bool OfflineMode { get; set; }
        
        public CoinGeckoFetcher(UserSettingsService userSettings)
        {
            this.Client = new HttpClient();
            this.BaseURL = "https://api.coingecko.com/api/v3/";
            this.PageCounter = userSettings.UserSettings.CoinAmount / CoinGeckoPageSize;
            this.OfflineMode = false;
            this.GetCoinsAsync();
        }

        public void GetCoinsAsync()
        {
            // Reset the list before doing anything
            this.Coins = new List<CoinGeckoMarket>();

            // Prepare the request
            this.Client = new HttpClient();
            this.Client.BaseAddress = new Uri(this.BaseURL);
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Load the required pages (settings set by user)
            for (int i = 1; i < 1 + PageCounter; ++i)
            {
                HttpResponseMessage response;

                // Execute the request
                try
                {
                    response = this.Client.GetAsync($"coins/markets?vs_currency=usd&order=market_cap_desc&per_page={CoinGeckoPageSize}&page={i}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response to ActivityModel
                        string jsonString = response.Content.ReadAsStringAsync().Result;

                        Coins.AddRange(JsonConvert.DeserializeObject<List<CoinGeckoMarket?>>(jsonString));
                    }
                }
                catch(Exception ex)
                {
                    this.Coins = new List<CoinGeckoMarket>();
                    this.OfflineMode = true;
                }
            }
        }

        public async Task<CoinGeckoFullData> GetCoinData(string coinId)
        {
            // Prepare the request
            this.Client = new HttpClient();
            this.Client.BaseAddress = new Uri(this.BaseURL);
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Execute the request
            HttpResponseMessage response = await this.Client.GetAsync($"coins/{coinId}?localization=false&tickers=false&market_data=false&community_data=false&developer_data=false&sparkline=false");

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

        public async Task<string> GetSimplePrice(string coinId)
        {
            // Prepare the request
            this.Client = new HttpClient();
            this.Client.BaseAddress = new Uri(this.BaseURL);
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Execute the request
                HttpResponseMessage response = await this.Client.GetAsync($"simple/price?ids={coinId}&vs_currencies=usd&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false");

                // All fine, deserialize the result
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString.Split(":")[2].Replace("}", "").Replace(".", ",");
                }
            }
            catch (Exception ex)
            {
                // TODO : Logging the error
            }

            return "";
        }

        public async Task RefreshCoinList(int totalCoins)
        {
            this.PageCounter = totalCoins / CoinGeckoPageSize;
            GetCoinsAsync();
        }
    }
}
