using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public class StoreEditViewModel : BaseViewModel
    {
        private int _id;
        private Stores _store;
        
        private Workers _user;
        public ObservableCollection<Workers> Users { get; }
        public ObservableCollection<string> ContrAgents { get; }

        internal void OnAppearing()
        {
            IsBusy = true;
        }

        public ObservableCollection<Stores> Stores { get; }
        private IAlertService alertService;
        private string _contrAgent;
        public Command LoadAgentsCommand { get; }
        public Command LoadOrdersCommand { get; }
        public Command UpdateOrder { get; }
        public StoreEditViewModel()
        {
            Users = new ObservableCollection<Workers>();
            Stores = new ObservableCollection<Stores>();
            ContrAgents = new ObservableCollection<string>();
            alertService = DependencyService.Resolve<IAlertService>();

            Title = $"Данные о магазине";
            alertService = DependencyService.Resolve<IAlertService>();
            LoadAgentsCommand = new Command(async () => await GetContrAgentsAsync());
            LoadOrdersCommand = new Command(async () => await GetUserData());
            UpdateOrder = new Command(OnEditClickedAsync);
        }

        private async void OnEditClickedAsync(object obj)
        {
            try
            {
                await UpdateIndicationsAsync(Store);
            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message, 1);
            }
        }

        private async Task<Stores> UpdateIndicationsAsync(Stores request)
        {
            alertService.ShowToast("Идет обновление... Пожалуйста, подождите...", 1);
            IsBusy = true;
            HttpClient client = new HttpClient();
            var response = await client.PutAsync($"{GlobalSettings.HostUrl}api/store/{User.Login}/{User.Token}",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(request),
            Encoding.UTF8, "application/json"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                alertService.ShowToast("Ошибка при обновлении... Попробуйте позже...", 1);
                return null;
            }
            else
            {
                var res = JsonConvert.DeserializeObject<Stores>(response.Content.ReadAsStringAsync().Result);
                Store = res;
                return null;
            }
        }

        public string ContrAgent
        {
            get => _contrAgent;
            set => SetProperty(ref _contrAgent, value);
        }

        public Workers User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public Stores Store
        {
            get => _store;
            set => SetProperty(ref _store, value);
        }

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

       

        private async Task GetUserData()
        {
            try
            {
                Users.Clear();
                alertService.ShowToast("Загрузка...", 1f);
                ContrAgents.Clear();
                var list = await App.Database.GetUsersAsync();
                if (list.Count > 0)
                {
                    foreach (var user in list)
                    {
                        Users.Add(user);
                    }
                    User = Users[0];
                }
                Stores.Clear();
                IEnumerable<string> organizationsItems = await GetContrAgentsAsync(true);
                foreach (var item in organizationsItems)
                {
                    ContrAgents.Add(item);
                }
                HttpClient _tokenclient = new HttpClient();
                var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/store/root_get_stores/store/{Id}/{User.Login}/{User.Token}");
                var _jsonResults = JsonConvert.DeserializeObject<Stores>(_responseToken);
                
                Stores.Add(_jsonResults);
                Store = Stores[0];
          
                alertService.ShowToast("Данные получены...", 1f);

            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message, 1f);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<IEnumerable<string>> GetContrAgentsAsync(bool forceRefresh = false)
        {
            try
            {

                HttpClient _tokenclient = new HttpClient();
                var url = $"{GlobalSettings.HostUrl}api/store/root_get_stores/agents/{Id}/{User.Login}/{User.Token}";
                var _responseToken = await _tokenclient.GetStringAsync(url);
                var _jsonResults = JsonConvert.DeserializeObject<List<string>>(_responseToken);
                return await Task.FromResult(_jsonResults.Distinct());
            }
            catch (Exception ex)
            {
                await alertService.ShowMessage("error", ex.Message);
                return null;
            }

        }
    }
}
