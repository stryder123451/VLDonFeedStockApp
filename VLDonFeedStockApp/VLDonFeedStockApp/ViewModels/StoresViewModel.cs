using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    public class StoresViewModel : BaseViewModel
    {
        public Command LoadStoresCommand { get; }
        public Command<Stores> EditStore { get; }
        public Command ChangeMoneyCourse { get; }

        private Stores _selectedStore;
        private IAlertService alertService; 
        public ObservableCollection<Stores> StoresList { get; }
        public ObservableCollection<Workers> WorkersList { get; }
        public StoresViewModel()
        {
            Title = "Магазины";
            StoresList = new ObservableCollection<Stores>();
            //EasyOrdersList = new ObservableCollection<Order>();
            WorkersList = new ObservableCollection<Workers>();
            //Title = $"Созданные заявки,{DateTime.Now.Month}, {DateTime.Now.Year}г";
            alertService = DependencyService.Resolve<IAlertService>();
            LoadStoresCommand = new Command(async () => await GetUserData());
            EditStore = new Command<Stores>(OnItemSelected);
            ChangeMoneyCourse = new Command(OnEditMoneyAsync);
        }

        private async void OnEditMoneyAsync(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(ContrAgentsPrices)}");
        }

        

        public Stores SelectedOrder
        {
            get => _selectedStore;
            set
            {
                SetProperty(ref _selectedStore, value);
                OnItemSelected(value);
            }
        }
        async void OnItemSelected(Stores item)
        {
            if (item == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(StoreEditPage)}?{nameof(StoreEditViewModel.Id)}={item.Id}");
        }

        internal void OnAppearing()
        {
            IsBusy = true;
        }

        private async Task GetUserData()
        {
            try
            {
                alertService.ShowToast("Получение показаний...", 1f);
                StoresList.Clear();
                WorkersList.Clear();
                var list = await App.Database.GetUsersAsync();
                if (list.Count > 0)
                {
                    foreach (var user in list)
                    {
                        WorkersList.Add(user);
                    }
                    HttpClient _tokenclient = new HttpClient();
                    _tokenclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WorkersList[0].Token);
                    var _responseToken = await _tokenclient.GetStringAsync($"" +
                        $"{GlobalSettings.HostUrl}api/store/root_get_stores/{WorkersList[0].Login}/{WorkersList[0].UserToken}");
                    var _jsonResults = JsonConvert.DeserializeObject<List<Stores>>(_responseToken);
                    foreach (var x in _jsonResults)
                    {
                        StoresList.Add(x);
                    }
                    
                    alertService.ShowToast("Данные получены...", 1f);
                }
                else
                {
                    alertService.ShowToast("Авторизируйтесь...", 1f);
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            }
            catch (Exception ex)
            {
                await alertService.ShowMessage("Панель администратора", "У вас недостаточно прав!!!");
                await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
