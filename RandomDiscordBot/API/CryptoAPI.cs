using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RandomDiscordBot.API
{
    internal class CryptoAPI
    {
        private static string CRYPTO_API_KEY = "YOUR_PRO_API_KEY";
        private string coin { get; set; }
        public CryptoAPI(string coin) 
        {
            this.coin = coin;
        }

        public string getPrice()
        {
            try
            {
                var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/tools/price-conversion");
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["amount"] = "1";
                queryString["symbol"] = coin;

                URL.Query = queryString.ToString();
                var client = new WebClient();
                client.Headers.Add("X-CMC_PRO_API_KEY", CRYPTO_API_KEY);
                client.Headers.Add("Accepts", "application/json");
                return client.DownloadString(URL.ToString());
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
