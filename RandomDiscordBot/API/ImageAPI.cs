using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RandomDiscordBot.API
{
    public class ImageApi
    {
        private string IMAPE_API_ACCESS_KEY = "YOUR_ACCESS_KEY";
        private string PRO_API_KEY = "YOUR_API_KEY";
        private string quote { get; set; }
        private int page {  get; set; }
        public ImageApi(string quote, int page)
        {
            this.quote = quote;
            this.page = page;
        }

        public string getImage()
        {
            try
            {
                var URL = new UriBuilder("https://api.unsplash.com/search/photos");
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["query"] = quote;
                queryString["page"] = page.ToString();
                queryString["per_page"] = "10";
                queryString["client_id"] = IMAPE_API_ACCESS_KEY;
                URL.Query = queryString.ToString();
                var client = new WebClient();
                client.Headers.Add("X-CMC_PRO_API_KEY", PRO_API_KEY);
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
