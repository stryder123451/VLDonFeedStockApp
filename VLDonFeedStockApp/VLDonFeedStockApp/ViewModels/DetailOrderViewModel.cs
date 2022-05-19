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
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public class DetailOrderViewModel : BaseViewModel
    {
        private int _id;
        //private string _name;
        //private string _description;
        //private string _state;
        //private string _materials;
        private IAlertService alertService;
        private Request _request;
        public Command LoadOrdersCommand { get; }
        public Command UpdateOrder { get; }
        public Command UpdateState { get; }
        public ObservableCollection<Request> Requests { get; }
        public DetailOrderViewModel()
        {
            Requests = new ObservableCollection<Request>();
            Title = $"Данные о заявке";
            alertService = DependencyService.Resolve<IAlertService>();
            LoadOrdersCommand = new Command(async () => await GetUserData());
            UpdateOrder = new Command(OnEditClicked);
            UpdateState = new Command(OnStateEditClicked);
        }
        private async void OnEditClicked(object obj)
        {
            try
            {
                await UpdateIndicationsAsync(Request);
            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message, 1);
            }


        }
        private async void OnStateEditClicked(object obj)
        {
            try
            {
                await UpdateStateAsync(Request);
            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message, 1);
            }


        }
        private async Task GetUserData()
        {
            try
            {
                //alertService.ShowToast("Получение данных о заказе...", 1f);
                Requests.Clear();
                HttpClient _tokenclient = new HttpClient();
                var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/order/get/{Id}");
                var _jsonResults = JsonConvert.DeserializeObject<Request>(_responseToken);
                _jsonResults.RuState = RuState(_jsonResults.State);
                Requests.Add(_jsonResults);
                Request = Requests[0];
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

        string RuState(string data)
        {
            var res = string.Empty;
            switch (data)
            {
                case "created":
                    res = "Создан";
                    break;
                case "actual":
                    res = "Активен";
                    break;
                case "weighted":
                    res = "Взвешен";
                    break;
                case "finished":
                    res = "Завершен";
                    break;
            }
            return res;
        }

        public Request Request
        {
            get => _request;
            set => SetProperty(ref _request, value);
        }

        public int Id
        {
            get=> _id;
            set=> SetProperty(ref _id, value);
        }
        //public string Name
        //{
        //    get => _name;
        //    set => SetProperty(ref _name, value);
        //}
        //public string Description
        //{
        //    get => _name;
        //    set => SetProperty(ref _name, value);
        //}
        //public string State
        //{
        //    get => _state;
        //    set => SetProperty(ref _state, value);
        //}
        //public string Materials
        //{
        //    get => _materials;
        //    set => SetProperty(ref _materials, value);
        //}

        internal void OnAppear()
        {
            IsBusy = true;
        }

        public async Task<Request> UpdateIndicationsAsync(Request indications)
        {
            alertService.ShowToast("Идет обновление... Пожалуйста, подождите...", 1);
            IsBusy = true;
            HttpClient client = new HttpClient();
            var response = await client.PutAsync($"{GlobalSettings.HostUrl}api/order",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(indications),
            Encoding.UTF8, "application/json"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                alertService.ShowToast("Ошибка при обновлении... Попробуйте позже...", 1);
                return null;
            }
            else
            {
                var res = JsonConvert.DeserializeObject<Request>(response.Content.ReadAsStringAsync().Result);
                res.RuState = RuState(res.State);
                Request = res;
                //await Shell.Current.GoToAsync($"//{nameof(LightIndicationsPage)}");
                return null;
            }
        }

        public async Task<Request> UpdateStateAsync(Request indications)
        {
            alertService.ShowToast("Идет обновление... Пожалуйста, подождите...", 1);
            IsBusy = true;
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{GlobalSettings.HostUrl}api/order/update_state/{indications.Id}",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(indications),
            Encoding.UTF8, "application/json"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                alertService.ShowToast("Ошибка при обновлении... Попробуйте позже...", 1);
                return null;
            }
            else
            {
                var res = JsonConvert.DeserializeObject<Request>(response.Content.ReadAsStringAsync().Result);
                res.RuState = RuState(res.State);
                Request = res;
                //await Shell.Current.GoToAsync($"//{nameof(LightIndicationsPage)}");
                return null;
            }
        }
    }
}
