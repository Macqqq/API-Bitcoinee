using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace API_Bitcoin
{
    // Assurez-vous que la classe Result implémente INotifyPropertyChanged
    public class Result : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _price;
        private double _priceChange1h;
        private double _priceChange24h;
        private double _priceChange7d;
        private double _marketCap;
        private double _volume24h;
        private double _circulatingSupply;
        private string _name;

        public double price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public double priceChange1h
        {
            get => _priceChange1h;
            set => SetProperty(ref _priceChange1h, value);
        }

        public double priceChange24h
        {
            get => _priceChange24h;
            set => SetProperty(ref _priceChange24h, value);
        }

        public double priceChange7d
        {
            get => _priceChange7d;
            set => SetProperty(ref _priceChange7d, value);
        }

        public double marketCap
        {
            get => _marketCap;
            set => SetProperty(ref _marketCap, value);
        }

        public double volume24h
        {
            get => _volume24h;
            set => SetProperty(ref _volume24h, value);
        }

        public double circulatingSupply
        {
            get => _circulatingSupply;
            set => SetProperty(ref _circulatingSupply, value);
        }

        public string name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        public Result BitcoinData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            BitcoinData = new Result { priceChange1h = 2.5 }; // Valeur statique pour tester
            DataContext = BitcoinData;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(5) // Refresh every 5 minutes
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            Task.Run(() => GetBitcoin()); // Call the method asynchronously
        }
        public class Root
        {
            [JsonProperty("result")]
            public List<Result> Result { get; set; }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => GetBitcoin()); // Call the method asynchronously
        }

        public async Task GetBitcoin()
        {
            try
            {
                var client = new RestClient("https://openapiv1.coinstats.app/coins");
                var request = new RestRequest();
                request.AddHeader("accept", "application/json");
                request.AddHeader("X-API-KEY", "0qSRvDzjqVmDWRMnBiHkx/2v/wefOSm6q8WIUguCrEE="); // Make sure to use your actual API key
                var response = await client.GetAsync(request);

                // Deserialize the JSON response
                var data = JsonConvert.DeserializeObject<Root>(response.Content);

                var bitcoin = data?.Result?.FirstOrDefault(coin => coin.name == "Bitcoin");

                if (bitcoin != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        BitcoinData.price = bitcoin.price;
                        BitcoinData.priceChange1h = bitcoin.priceChange1h;
                        BitcoinData.priceChange24h = bitcoin.priceChange24h;
                        BitcoinData.priceChange7d = bitcoin.priceChange7d;
                        BitcoinData.marketCap = bitcoin.marketCap;
                        BitcoinData.volume24h = bitcoin.volume24h;
                        BitcoinData.circulatingSupply = bitcoin.circulatingSupply;
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
