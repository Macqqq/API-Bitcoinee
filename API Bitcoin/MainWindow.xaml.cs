using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace API_Bitcoin
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            GetBitcoin();
        }
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Coin
        {
            public string id { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
            public string symbol { get; set; }
            public int rank { get; set; }
            public double price { get; set; }
            public double priceBtc { get; set; }
            public double volume { get; set; }
            public double marketCap { get; set; }
            public object availableSupply { get; set; }
            public object totalSupply { get; set; }
            public double priceChange1h { get; set; }
            public double priceChange1d { get; set; }
            public double priceChange1w { get; set; }
            public string websiteUrl { get; set; }
            public string twitterUrl { get; set; }
            public List<string> exp { get; set; }
            public string contractAddress { get; set; }
            public int? decimals { get; set; }
        }

        public class Root
        {
            public List<Coin> coins { get; set; }
            public string warning { get; set; }
        }

        public async Task GetBitcoin()
        {
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://api.coinstats.app/public/v1/coins?skip=0&limit=5&currency=EUR");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(result);

                    if (myDeserializedClass.coins != null && myDeserializedClass.coins.Any())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Nomcrypto1.Text = myDeserializedClass.coins[0].name;
                            TB_BitcoinPrice.Text = myDeserializedClass.coins[0].price.ToString("F2");
                            Dimcrypto1.Text = "N'" + myDeserializedClass.coins[0].rank.ToString();
                            TB_BitcoinChange.Text = myDeserializedClass.coins[0].priceChange1d.ToString("F2") + ("%");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    TB_Bitcoin.Text = $"Erreur : {ex.Message}";
                });
            }
        }
    }
}
