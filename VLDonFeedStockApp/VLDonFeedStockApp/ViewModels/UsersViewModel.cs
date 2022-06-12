using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        private Workers _user;
        IAlertService alertService;
        public ObservableCollection<Workers> Users { get; }
        public ObservableCollection<Workers> RegisteredUsers { get; }
        public Command LoadUsersCommand { get; }
        public Command<Workers> EditOrder { get; }

        public UsersViewModel()
        {
            alertService = DependencyService.Resolve<IAlertService>();
            Title = "Пользователи";
            Users = new ObservableCollection<Workers>();
            RegisteredUsers = new ObservableCollection<Workers>();
            LoadUsersCommand = new Command(async () => await GetUserData());
            EditOrder = new Command<Workers>(OnItemSelected);
        }

         void OnItemSelected(Workers item)
        {
            if (item == null)
                return;
            AcceptUser(item);

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

        public Workers User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }


        private async void AcceptUser(Workers workers)
        {
            try
            {
                HttpClient _tokenClientPrice = new HttpClient();
                var _responseTokenPrice = await _tokenClientPrice.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/accept/{User.Login}/{User.Token}/{workers.Login}");
                IsBusy = true;
            }
            catch (Exception ex)
            {
                
            }
        }


        private async Task GetUserData()
        {

            try
            {
                RegisteredUsers.Clear();
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
                if (User.Role.Length != "root".Length)
                {
                    await alertService.ShowMessage("Администратор", "У вас недостаточно прав!!!");
                    await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                }
                else
                {
                    //else
                    //{
                    //    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                    //}
                    HttpClient _tokenClientPrice = new HttpClient();
                    var _responseTokenPrice = await _tokenClientPrice.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/users/{User.Login}/{User.Token}");
                    var _jsonResultsPrice = JsonConvert.DeserializeObject<List<Workers>>(_responseTokenPrice);
                    foreach (var price in _jsonResultsPrice)
                    {
                        RegisteredUsers.Add(price);
                    }
                }
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

        internal void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
