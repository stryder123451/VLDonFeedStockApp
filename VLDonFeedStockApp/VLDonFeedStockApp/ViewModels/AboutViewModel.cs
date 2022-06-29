using Newtonsoft.Json;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        IAlertService alertService;

        private Workers _user;
        private bool _isAdmin;
        private string _status;
        private string _verison;
        private int _all;
        private int _created;
        private int _actual;
        private int _weighted;
        private int _finished;
        public ObservableCollection<Workers> Users { get; }
        public ObservableCollection<Order> Orders { get; }
        public Workers User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }
        public AboutViewModel()
        {
            alertService = DependencyService.Resolve<IAlertService>();
            Title = "Данные о пользователе";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            Users = new ObservableCollection<Workers>();
            Orders = new ObservableCollection<Order>();
            LoadItemsCommand = new Command(async () => await GetUserData());
            
        }
       
        public int All
        {
            get => _all;
            set => SetProperty(ref _all, value);
        }
        public string Version
        {
            get => _verison;
            set => SetProperty(ref _verison, value);
        }
        public int Created
        {
            get => _created;
            set => SetProperty(ref _created, value);
        }
        public int Actual
        {
            get => _actual;
            set => SetProperty(ref _actual, value);
        }
        public int Weighted
        {
            get => _weighted;
            set => SetProperty(ref _weighted, value);
        }
        public int Finished
        {
            get => _finished;
            set => SetProperty(ref _finished, value);
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set => SetProperty(ref _isAdmin, value);
        }
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        private async Task GetUserData()
        {
            try
            {
                Users.Clear();
                alertService.ShowToast("Загрузка...", 1f);
                Actual = 0;
                Weighted = 0;
                Finished = 0;
                Created = 0;
                All = 0;
                var list = await App.Database.GetUsersAsync();
                if (list.Count > 0)
                {
                    foreach (var user in list)
                    {
                        Users.Add(user);
                    }
                    Users[0].RuRole = SetRole(Users[0].Role);
                    User = Users[0];
                    if (User.IsAccepted)
                    {
                        Status = "Подтвержден";
                    }
                    else
                    {
                        Status = "Не подтвержден";
                    }
                    HttpClient _tokenClientPrice = new HttpClient();
                    _tokenClientPrice.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",User.Token);
                    var _responseTokenPrice = await _tokenClientPrice.GetStringAsync($"{GlobalSettings.HostUrl}api/price/Version");
                    var _jsonResultsPrice = JsonConvert.DeserializeObject<Prices>(_responseTokenPrice);
                    Version = $"Версия приложения: {_jsonResultsPrice.Version}";
                    if (decimal.Parse(_jsonResultsPrice.Version.Replace('.', ',')) != decimal.Parse(GlobalSettings.Version.Replace('.', ',')))
                    {
                        await alertService.ShowMessage("Версия", "Обновите приложения до актуальной версии!!!");
                        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                    }
                    else
                    {
                        alertService.ShowToast($"Здравствуйте, {Users[0].Name}!!!", 1f);
                        switch (User.RuRole)
                        {
                            case "Директор":
                                await DirectorLogin();

                                break;
                            case "КонтрАгент":
                                await ContrAgentLogin();

                                break;
                            case "Администратор":
                                await RootLogin();
                                break;
                        }

                        try
                        {
                            Orders.Clear();
                            HttpClient _tokenclient = new HttpClient();
                            _tokenclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
                            var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/order/created/{User.Login}/{User.UserToken}");
                            var _jsonResults = JsonConvert.DeserializeObject<List<Order>>(_responseToken);
                            foreach (var x in _jsonResults)
                            {
                                Orders.Add(x);
                            }

                            Created = Orders.Count;

                            Orders.Clear();

                            HttpClient _tokenclientActual = new HttpClient();
                            _tokenclientActual.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
                            var _responseTokenActual = await _tokenclientActual.GetStringAsync($"{GlobalSettings.HostUrl}api/order/active/{User.Login}/{User.UserToken}");
                            var _jsonResultsActual = JsonConvert.DeserializeObject<List<Order>>(_responseTokenActual);
                            foreach (var x in _jsonResultsActual)
                            {
                                Orders.Add(x);
                            }
                            Actual = Orders.Count;

                            Orders.Clear();

                            HttpClient _tokenclientWeighted = new HttpClient();
                            _tokenclientWeighted.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
                            var _responseTokenWeighted = await _tokenclientWeighted.GetStringAsync($"{GlobalSettings.HostUrl}api/order/weighted/{User.Login}/{User.UserToken}");
                            var _jsonResultsWeighted = JsonConvert.DeserializeObject<List<Order>>(_responseTokenWeighted);
                            foreach (var x in _jsonResultsWeighted)
                            {
                                Orders.Add(x);
                            }
                            Weighted = Orders.Count;


                            Orders.Clear();

                            HttpClient _tokenclientFinished = new HttpClient();
                            _tokenclientFinished.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
                            var _responseTokenFinished = await _tokenclientWeighted.GetStringAsync($"{GlobalSettings.HostUrl}api/order/finished/{User.Login}/{User.UserToken}");
                            var _jsonResultsFinished = JsonConvert.DeserializeObject<List<Order>>(_responseTokenFinished);
                            foreach (var x in _jsonResultsFinished)
                            {
                                Orders.Add(x);
                            }
                            Finished = Orders.Count;

                            All = Created + Actual + Finished + Weighted;
                        }
                        catch (Exception ex)
                        {
                            await alertService.ShowMessage("Регистрация", "Ваша учетная запись не подтверждена,обратитесь к администратору!!!");
                        }
                    }
                }

                else
                {
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
                
                //MessagingCenter.Subscribe<LoginViewModel>(this, "root", (sender) =>
                //{
                //    IsAdmin = true;
                //});
                //MessagingCenter.Subscribe<LoginViewModel>(this, "employee", (sender) =>
                //{
                //    AboutViewModel aboutViewModel = new AboutViewModel(false);
                //});
                //MessagingCenter.Subscribe<LoginViewModel>(this, "admin", (sender) =>
                //{
                //    AboutViewModel aboutViewModel = new AboutViewModel(false);
                //});

                //CrossFirebasePushNotification.Current.Subscribe("all");
            }
            catch (Exception ex)
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            finally
            {
                IsBusy = false;

            }
        }

        private async Task ContrAgentLogin()
        {
            //HttpClient _tokenclientDiff = new HttpClient();
            //var _responseTokenDiff = await _tokenclientDiff.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/related_organization/{User.Organization}");
            HttpClient _channelclients = new HttpClient();
            var _responseChannelClients = await _channelclients.GetStringAsync(
                $"{GlobalSettings.HostUrl}api/auth/related_organization_channels/{User.Organization}");
            string _channel = _responseChannelClients;
            if (CrossFirebasePushNotification.Current.SubscribedTopics.Length >= 0)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
                
                CrossFirebasePushNotification.Current.Subscribe(_channel);
                
                CrossFirebasePushNotification.Current.Subscribe($"{User.Login}");
            }
        }
        private async Task RootLogin()
        {
            HttpClient _channelclients = new HttpClient();
            var _responseChannelClients = await _channelclients.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/related_organization_channels");
            List<string> _channels = JsonConvert.DeserializeObject<List<string>>(_responseChannelClients);
            if (CrossFirebasePushNotification.Current.SubscribedTopics.Length >= 0)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
                foreach (var x in _channels)
                {
                    CrossFirebasePushNotification.Current.Subscribe(x);
                }
                CrossFirebasePushNotification.Current.Subscribe($"root");
            }
           
        }

        private async Task DirectorLogin()
        {
            HttpClient _tokenclientDiff = new HttpClient();
            var _responseTokenDiff = await _tokenclientDiff.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/store/{User.Organization}/{User.Address}");
            if (CrossFirebasePushNotification.Current.SubscribedTopics.Length >= 0)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
                CrossFirebasePushNotification.Current.Subscribe($"{_responseTokenDiff}");
                if (User.RuRole == "Директор")
                {
                    CrossFirebasePushNotification.Current.Subscribe($"{_responseTokenDiff}_weighted");
                }
                if (CrossFirebasePushNotification.Current.SubscribedTopics.Length == 0)
                {
                    CrossFirebasePushNotification.Current.Subscribe(_responseTokenDiff);
                }
                CrossFirebasePushNotification.Current.Subscribe($"{User.Login}");
            }
        }

        public string SetRole(string _data)
        {
            switch (_data)
            {
                case "admin":
                    _data = "Директор";
                    break;
                case "root":
                    _data = "Администратор";
                    break;
                default: 
                    _data = "КонтрАгент";
                        break;
            }
            return _data;
        }
        public ICommand OpenWebCommand { get; }
        public Command LoadItemsCommand { get; }
        internal void OnAppearing()
        {
            IsBusy = true;
        }
    }
}