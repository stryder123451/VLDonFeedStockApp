using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;
using Organizations = VLDonFeedStockApp.Models.Organizations;

namespace VLDonFeedStockApp.ViewModels
{
    [QueryProperty(nameof(ContrAgent), nameof(ContrAgent))]
    public class MoneyCourseViewModel : BaseViewModel
    {
        IAlertService alertService;

        private string _contrAgent;
        public Command LoadItemsCommand { get; }
        public Command UpdateOrder { get; }
        public Command BackCommand { get; }
        
        private Prices _prices;
        public Prices Prices
        {
            get => _prices;
            set => SetProperty(ref _prices, value);
        }
        public string ContrAgent
        {
            get => _contrAgent;
            set => SetProperty(ref _contrAgent, value);
        }

        public MoneyCourseViewModel()
        {
            //Requests = new ObservableCollection<Request>();
            //Users = new ObservableCollection<Workers>();
            Title = $"Цена за кг";
            alertService = DependencyService.Resolve<IAlertService>();
            LoadItemsCommand = new Command(async () => await GetUserData());
            UpdateOrder = new Command(OnEditClicked);
            BackCommand = new Command(OnCancel);
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync($"//{nameof(StoresPage)}");
        }

        internal void OnAppearing()
        {
            IsBusy = true;
        }

        private async void OnEditClicked(object obj)
        {
            try
            {
                await ChangeData(Prices);
            }
            catch (Exception ex)
            {
                alertService.ShowToast($"{ex.Message}", 1);
            }
        }

        private async Task<Prices> ChangeData(Prices indications)
        {

            alertService.ShowToast("Идет обновление... Пожалуйста, подождите...", 1);
            IsBusy = true;
            HttpClient client = new HttpClient();
            var response = await client.PutAsync($"{GlobalSettings.HostUrl}api/price",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(indications),
            Encoding.UTF8, "application/json"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                alertService.ShowToast("Ошибка при обновлении... Попробуйте позже...", 1);
                return null;
            }
            else
            {
                var res = JsonConvert.DeserializeObject<Prices>(response.Content.ReadAsStringAsync().Result);

                Prices = res;


                //await Shell.Current.GoToAsync($"//{nameof(LightIndicationsPage)}");
                return null;
            }

        }

        private async Task GetUserData()
        {
            try
            {

                alertService.ShowToast("Загрузка...", 1f);

                //var list = await App.Database.GetUsersAsync();
                //if (list.Count > 0)
                //{
                //    foreach (var user in list)
                //    {
                //        Users.Add(user);
                //    }
                //    User = Users[0];
                //}

                HttpClient _tokenClientPrice = new HttpClient();
                var _responseTokenPrice = await _tokenClientPrice.GetStringAsync($"{GlobalSettings.HostUrl}api/price/{ContrAgent}");
                var _jsonResultsPrice = JsonConvert.DeserializeObject<Prices>(_responseTokenPrice);
                Prices = _jsonResultsPrice;


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
    }

}
