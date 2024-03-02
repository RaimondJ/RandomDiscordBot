using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomDiscordBot.API
{
    public class OpenAI
    {
        private const string API_KEY = "YOUR_OPENAI_API_KEY";

        private string prompt {  get; set; }    

        public OpenAI(string prompt)
        {
            this.prompt = prompt;
        }

        public string generateImage()
        {
            try
            {
                var client = new RestClient("https://api.openai.com");
                var request = new RestRequest("v1/images/generations");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + API_KEY);
                var myData = new
                {
                    model = "dall-e-3",
                    prompt = prompt,
                    n = 1,
                    size = "1024x1024",
                };
                string jsonData = JsonConvert.SerializeObject(myData);
                request.AddJsonBody(jsonData);
                var response = client.Post(request);
                var content = response.Content;
                Console.WriteLine(response.IsSuccessful);
                return content;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
