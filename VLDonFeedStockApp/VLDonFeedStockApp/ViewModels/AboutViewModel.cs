using System;
using System.Collections.ObjectModel;
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
                    User = Users[0];
                }
                else
                {
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
                alertService.ShowToast($"Здравствуйте, {Users[0].Name}!!!", 1f);
                //HttpClient _tokenclientDiff = new HttpClient();
                //var _responseTokenDiff = await _tokenclientDiff.GetStringAsync($"http://velikihs-001-site1.dtempurl.com/api/user/{Users[0].City}/{Users[0].Street}/{Users[0].HouseNumber}/{Users[0].Login}");
                //if (CrossFirebasePushNotification.Current.SubscribedTopics.Length > 0)
                //{
                //    CrossFirebasePushNotification.Current.UnsubscribeAll();
                //    CrossFirebasePushNotification.Current.Subscribe(_responseTokenDiff);
                //}
                //if (CrossFirebasePushNotification.Current.SubscribedTopics.Length == 0)
                //{
                //    CrossFirebasePushNotification.Current.Subscribe(_responseTokenDiff);
                //}

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

        public ICommand OpenWebCommand { get; }
        public Command LoadItemsCommand { get; }
        internal void OnAppearing()
        {
            IsBusy = true;
        }
    }
}