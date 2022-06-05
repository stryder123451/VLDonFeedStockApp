using Newtonsoft.Json;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
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
        public ObservableCollection<Workers> Users { get; }
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
            LoadItemsCommand = new Command(async () => await GetUserData());
            MessagingCenter.Subscribe<LoginViewModel>(this, "root", (sender) =>
            {
                  IsAdmin = true;
            });
            MessagingCenter.Subscribe<LoginViewModel>(this, "employee", (sender) =>
            {
                IsAdmin = false;
            });
            MessagingCenter.Subscribe<LoginViewModel>(this, "admin", (sender) =>
            {
                IsAdmin = false;
            });
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set => SetProperty(ref _isAdmin, value);
        }
        private async Task GetUserData()
        {
            try
            {
                Users.Clear();
                alertService.ShowToast("Загрузка...", 1f);

                var list = await App.Database.GetUsersAsync();
                if (list.Count > 0)
                {
                    foreach (var user in list)
                    {
                        Users.Add(user);
                    }
                    Users[0].RuRole = SetRole(Users[0].Role);
                    User = Users[0];
                }

                else
                {
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
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
            HttpClient _tokenclientDiff = new HttpClient();
            var _responseTokenDiff = await _tokenclientDiff.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/related_organization/{User.Organization}");
            HttpClient _channelclients = new HttpClient();
            var _responseChannelClients = await _channelclients.GetStringAsync(
                $"{GlobalSettings.HostUrl}api/auth/related_organization_channels/{_responseTokenDiff}");
            List<string> _channels = JsonConvert.DeserializeObject<List<string>>(_responseChannelClients);
            if (CrossFirebasePushNotification.Current.SubscribedTopics.Length > 0)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
                foreach (var x in _channels)
                {
                    CrossFirebasePushNotification.Current.Subscribe(x);
                }
            }
        }
        private async Task RootLogin()
        {
            HttpClient _channelclients = new HttpClient();
            var _responseChannelClients = await _channelclients.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/related_organization_channels");
            List<string> _channels = JsonConvert.DeserializeObject<List<string>>(_responseChannelClients);
            if (CrossFirebasePushNotification.Current.SubscribedTopics.Length > 0)
            {
                CrossFirebasePushNotification.Current.UnsubscribeAll();
                foreach (var x in _channels)
                {
                    CrossFirebasePushNotification.Current.Subscribe(x);
                }
            }
        }

        private async Task DirectorLogin()
        {
            HttpClient _tokenclientDiff = new HttpClient();
            var _responseTokenDiff = await _tokenclientDiff.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/store/{User.Organization}/{User.Address}");
            if (CrossFirebasePushNotification.Current.SubscribedTopics.Length > 0)
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