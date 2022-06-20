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
    public class ContrAgentsPricesViewModel : BaseViewModel
    {
        public Command LoadStoresCommand { get; }
        public Command<Prices> EditOrder { get; }
        public Command ChangeMoneyCourse { get; }
        public Command<Prices> EditStore { get; }
        private Prices _selectedPrice;
        private IAlertService alertService;
        public ObservableCollection<Prices> StoresList { get; }

        internal void OnAppearing()
        {
            IsBusy = true;
        }

        public ObservableCollection<Workers> WorkersList { get; }
        public ContrAgentsPricesViewModel()
        {
            Title = "КонтрАгенты";
            StoresList = new ObservableCollection<Prices>();
            //EasyOrdersList = new ObservableCollection<Order>();
            WorkersList = new ObservableCollection<Workers>();
            //Title = $"Созданные заявки,{DateTime.Now.Month}, {DateTime.Now.Year}г";
            alertService = DependencyService.Resolve<IAlertService>();
            LoadStoresCommand = new Command(async () => await GetUserData());
            EditOrder = new Command<Prices>(OnItemSelectedAsync);
            ChangeMoneyCourse = new Command(OnEditContrAgentAsync);
        }

        private async void OnEditContrAgentAsync(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(StoresPage)}");
        }
        public Prices SelectedOrder
        {
            get => _selectedPrice;
            set
            {
                SetProperty(ref _selectedPrice, value);
                OnItemSelectedAsync(value);
            }
        }
        private async void OnItemSelectedAsync(Prices obj)
        {
            if (obj == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(MoneyCourse)}?{nameof(MoneyCourseViewModel.ContrAgent)}={obj.RelatedContrAgent}");
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
                    var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/price/full/{WorkersList[0].Login}/{WorkersList[0].UserToken}");
                    var _jsonResults = JsonConvert.DeserializeObject<List<Prices>>(_responseToken);
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
