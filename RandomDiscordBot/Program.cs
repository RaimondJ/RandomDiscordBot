using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomDiscordBot.API;
using RestSharp;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static RandomDiscordBot.GlobalFunctions;

namespace RandomDiscordBot
{
    class Program
    {
        private const string DISCORD_BOT_KEY = "YOUR_BOT_TOKEN";
        private Random rand { get; set; } = new Random();
        private Dictionary<string, Command> commands = new Dictionary<string, Command>();
        private readonly DiscordSocketClient _client;

        static void Main(string[] args)
            => new Program()
                .MainAsync()
                .GetAwaiter()
                .GetResult();

        public Program()
        {
            buildCommands();
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            _client = new DiscordSocketClient(config);
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += CheckMessages;
        }

        private void buildCommands()
        {
            commands.Add(".randomimage", new Command(2, "Kasuta: .randomimage <mingi quote>"));
            commands.Add(".riik", new Command (2, "Kasuta: .riik <riigi nimi>"));
            commands.Add(".joonista", new Command (2, "Kasuta: .joonista <mingi tekst>"));
            commands.Add(".btcprice", new Command (1, ""));
            commands.Add(".ethprice", new Command (1, "")   );
            commands.Add(".norris", new Command (1, ""));
            commands.Add(".help", new Command (1, ""));
        }

        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, DISCORD_BOT_KEY);
            await _client.StartAsync();
            await _client.SetGameAsync(".help", "", ActivityType.Playing);
            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");
            return Task.CompletedTask;
        }

        private async Task CheckMessages(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null)
                return;
            if (message.Author.IsBot)
                return;
            string str = message.Content;
            string[] vals = explode(str, " ");
            string command = vals[0];
            command = command.ToLowerInvariant();
            if (str.Length >= 2)
            {
                if (str.EndsWith(" "))
                {
                    str = str.Remove(str.Length - 1, 1);
                    vals = explode(str, " ");
                }
            }
            if ((command == ".joonista") && commands[command].varLength <= vals.Length)
            {
                string[] newvals = new string[commands[command].varLength];
                newvals[0] = command;
                newvals[1] = str.Remove(0, command.Length + 1);
                vals = newvals;
            }
            if ((command == ".randomimage") && commands[command].varLength <= vals.Length)
            {
                string[] newvals = new string[commands[command].varLength];
                newvals[0] = command;
                newvals[1] = str.Remove(0, command.Length + 1);
                vals = newvals;
            }
            if ((command == ".riik") && commands[command].varLength <= vals.Length)
            {
                string[] newvals = new string[commands[command].varLength];
                newvals[0] = command;
                newvals[1] = str.Remove(0, command.Length + 1);
                vals = newvals;
            }
            if (commands.ContainsKey(command))
            {
                if (commands[command].varLength == vals.Length || commands[command].varLength == 1)
                {
                    switch (command)
                    {
                        case ".help":
                            {
                                var builder = new EmbedBuilder()
                                {
                                    Color = Color.Red,
                                    Title = "Help"
                                };
                                string topGuilds = "";
                                int i = 0;
                                string commandss = "";
                                foreach (var commandd in commands)
                                {
                                    string cmd = commandd.Key;
                                    string usage = commandd.Value.usage;
                                    if (usage == "")
                                    {
                                        commandss += cmd + Environment.NewLine;
                                    }
                                    else
                                    {
                                        commandss += cmd + ": " + usage + Environment.NewLine;
                                    }
                                }
                                builder.AddField("Commandid", commandss, false);
                                await message.Channel.SendMessageAsync("", false, builder.Build());
                                break;
                            }
                        case ".riik":
                            {
                                try
                                {
                                    string country = vals[1];
                                    if (country.Length < 3)
                                    {
                                        await message.Channel.SendMessageAsync("Kirjuta vähemalt 3 tähemärki.");
                                        return;
                                    }
                                    WebClient web = new WebClient();
                                    JArray json = (JArray)JsonConvert.DeserializeObject(web.DownloadString("https://restcountries.com/v3.1/name/" + country), new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });

                                    JObject obj = (JObject)json[0];
                                    string common = (string)obj["name"]["common"];
                                    string official = (string)obj["name"]["official"];
                                    string flag = (string)obj["cca2"];
                                    bool unMember = (bool)obj["unMember"];
                                    bool independent = (bool)obj["independent"];
                                    string capital = (string)((JArray)obj["capital"])[0];
                                    string currency = "N/A";
                                    string region = (string)obj["region"];
                                    string subregion = (string)obj["subregion"];
                                    string languages = "";
                                    string area = (string)obj["area"];
                                    string population = (string)obj["population"];
                                    string timezones = "";
                                    string continents = "";
                                    foreach (var ob in obj["currencies"].Values())
                                    {
                                        currency = (string)ob["name"];
                                        break;
                                    }
                                    foreach (var ob in obj["languages"].Values())
                                    {
                                        languages += (string)ob + Environment.NewLine;
                                    }
                                    foreach (var val in (JArray)obj["timezones"])
                                        timezones += (string)val + Environment.NewLine;
                                    foreach (var val in (JArray)obj["continents"])
                                        continents += (string)val + Environment.NewLine;
                                    var builder = new EmbedBuilder()
                                    {
                                        Color = Color.Blue,
                                        Title = common + " :flag_" + flag.ToLower() + ":"
                                    };
                                    string topGuilds = "";
                                    int i = 0;
                                    builder.AddField("Common Name", common, false);
                                    builder.AddField("Official Name", official, false);
                                    builder.WithImageUrl((string)obj["flags"]["png"]);
                                    builder.AddField("UN Member", unMember.ToString(), true);
                                    builder.AddField("Independent", independent.ToString(), true);
                                    //builder.AddField("\u200B", "\u200B", false);
                                    builder.AddField("Capital City", capital, true);
                                    builder.AddField("Currency", currency, true);
                                    //builder.AddField("\u200B", "\u200B", false);
                                    builder.AddField("Region", region, true);
                                    builder.AddField("Sub Region", subregion, true);
                                    //builder.AddField("\u200B", "\u200B", false);
                                    builder.AddField("Area", FormatNumber(area) + " km2", true);
                                    builder.AddField("Population", FormatNumber(population), true);
                                    // builder.AddField("\u200B", "\u200B", false);
                                    builder.AddField("Languages", languages, false);
                                    builder.AddField("Timezones", timezones, false);
                                    builder.AddField("Continents", continents, false);

                                    await message.Channel.SendMessageAsync("", false, builder.Build());
                                }
                                catch (Exception ex)
                                {
                                    await message.Channel.SendMessageAsync("Riiki ei leitud :/");
                                }
                                break;
                            }
                        case ".btcprice":
                            {
                                CryptoAPI api = new CryptoAPI("BTC");
                                string json = api.getPrice();
                                JObject jObj = parseJson(json);
                                //Console.WriteLine(jObj);
                                if (jObj["data"]["quote"]["USD"]["price"] != null)
                                {
                                    double price = double.Parse(((string)jObj["data"]["quote"]["USD"]["price"]).Replace(".", ","));
                                    int price_int = (int)price;
                                    await message.Channel.SendMessageAsync("Bitcoin price: " + price_int + " USD.");
                                }
                                break;
                            }
                        case ".ethprice":
                            {
                                CryptoAPI api = new CryptoAPI("ETH");
                                string json = api.getPrice();
                                JObject jObj = parseJson(json);
                                if (jObj["data"]["quote"]["USD"]["price"] != null)
                                {
                                    double price = double.Parse(((string)jObj["data"]["quote"]["USD"]["price"]).Replace(".", ","));
                                    int price_int = (int)price;
                                    await message.Channel.SendMessageAsync("Ethereum price: " + price_int + " USD.");
                                }
                                break;
                            }
                        case ".norris":
                            {
                                WebClient web = new WebClient();
                                JObject json = parseJson(web.DownloadString("https://api.chucknorris.io/jokes/random"));
                                if (json["value"] != null)
                                {
                                    await message.Channel.SendMessageAsync((string)json["value"]);
                                }
                                break;
                            }
                        case ".randomimage":
                            {
                                List<string> links = new List<string>();
                                string quote = vals[1];
                                int page = 1;
                                ImageApi imageApi = new ImageApi(quote, page);
                                JObject json = parseJson(imageApi.getImage());
                                if (json["total_pages"] != null)
                                {
                                    if ((int)json["total_pages"] != 0)
                                    {
                                        page = rand.Next(1, int.Parse((string)json["total_pages"]));
                                        json = parseJson(imageApi.getImage());
                                    }
                                    else
                                    {
                                        await message.Channel.SendMessageAsync("Ei leidnud ühtegi pilti :/");
                                        return;
                                    }
                                }
                                if (json["results"] != null)
                                {
                                    JArray arr = json["results"] as JArray;
                                    foreach (JObject js in arr)
                                    {
                                        if (js["urls"]["raw"] != null)
                                            links.Add((string)js["urls"]["raw"]);
                                    }
                                    await message.Channel.SendMessageAsync(links[rand.Next(0, links.Count)]);
                                }
                                else
                                {
                                    await message.Channel.SendMessageAsync("Ei leidnud ühtegi pilti :/");
                                }
                                break;
                            }
                        case ".joonista":
                            {
                                await message.Channel.SendMessageAsync("Genereerin pilti...");
                                OpenAI openAI = new OpenAI(vals[1]);
                                JObject json = parseJson(openAI.generateImage());
                                if (json != null)
                                {
                                    if (json["data"] != null)
                                    {
                                        string link = "";
                                        JArray arr = json["data"] as JArray;
                                        foreach (JObject js in arr)
                                        {
                                            if (js["url"] != null)
                                                link = (string)js["url"];
                                        }
                                        var builder = new EmbedBuilder()
                                        {
                                            Color = Color.Blue,
                                            Title = vals[1]
                                        };
                                        builder.WithImageUrl(link);
                                        await message.Channel.SendMessageAsync("", false, builder.Build());
                                    }
                                    else
                                    {
                                        await message.Channel.SendMessageAsync("Tekkis viga :/");
                                    }
                                }
                                else
                                {
                                    await message.Channel.SendMessageAsync("Tekkis viga :/");
                                }
                                break;
                            }
                    }
                }
                else
                {
                    await message.Channel.SendMessageAsync(commands[command].usage);
                }
            }
        }
    }
}