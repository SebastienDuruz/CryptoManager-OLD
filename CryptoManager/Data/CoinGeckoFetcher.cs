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
            this.Coins = GetCoins();

            // Set offlineMode if CoinGecko is not reachable
            if(this.Coins.Count == 0)
                this.OfflineMode = true;
        }

        private List<CoinGeckoMarket> GetCoins()
        {
            List<CoinGeckoMarket> finalCoinsList = new List<CoinGeckoMarket>();

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

                        List<CoinGeckoMarket> coins = JsonConvert.DeserializeObject<List<CoinGeckoMarket?>>(jsonString);
                        finalCoinsList.AddRange(coins);
                    }
                }
                catch(Exception ex)
                {
                    // TODO : Logging the errors, maybe set Offline Mode ?
                }
            }

            return finalCoinsList;
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

        public string GetSimplePrice(string coinId)
        {
            // Prepare the request
            this.Client = new HttpClient();
            this.Client.BaseAddress = new Uri(this.BaseURL);
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Execute the request
                HttpResponseMessage response = this.Client.GetAsync($"simple/price?ids={coinId}&vs_currencies=usd&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false").Result;

                // All fine, deserialize the result
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString.Split(":")[2].Replace("}", "");
                }
            }
            catch (Exception ex)
            {
                
            }

            return "";
        }

        public List<Tuple<string, string>> GetSimplePrice(List<string> coinIds)
        {
            List<Tuple<string, string>> coinInfos = new List<Tuple<string, string>>();

            foreach(string coinId in coinIds)
            {
                coinInfos.Add(new Tuple<string, string>(coinId, GetSimplePrice(coinId)));
            }
            
            return coinInfos;
        }

        public void RefreshCoinList(int totalCoins)
        {
            this.PageCounter = totalCoins / CoinGeckoPageSize;
            this.Coins = GetCoins();
        }
    }
}
