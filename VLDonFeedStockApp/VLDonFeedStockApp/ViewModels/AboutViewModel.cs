using Plugin.FirebasePushNotification;
using System;
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
                HttpClient _tokenclientDiff = new HttpClient();
                var _responseTokenDiff = await _tokenclientDiff.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/store/{User.Organization}/{User.Address}");
                if (CrossFirebasePushNotification.Current.SubscribedTopics.Length > 0)
                {
                    CrossFirebasePushNotification.Current.UnsubscribeAll();
                    CrossFirebasePushNotification.Current.Subscribe($"{_responseTokenDiff}");
                }
                if (CrossFirebasePushNotification.Current.SubscribedTopics.Length == 0)
                {
                    CrossFirebasePushNotification.Current.Subscribe(_responseTokenDiff);
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