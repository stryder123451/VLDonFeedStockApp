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
    public class FinishedOrdersViewModel : BaseViewModel
    {
        public Command LoadOrdersCommand { get; }
        public Command<Order> EditOrder { get; }
        public ObservableCollection<Request> OrdersList { get; }
        public ObservableCollection<Order> EasyOrdersList { get; }
        public ObservableCollection<Workers> Workers { get; }
        private IAlertService alertService;
        private Order _selectedOrder;
        public Command AddItemCommand { get; }
        public FinishedOrdersViewModel()
        {
            OrdersList = new ObservableCollection<Request>();
            EasyOrdersList = new ObservableCollection<Order>();
            Workers = new ObservableCollection<Workers>();
            Title = $"Созданные заявки,{DateTime.Now.Month}, {DateTime.Now.Year}г";
            alertService = DependencyService.Resolve<IAlertService>();
            LoadOrdersCommand = new Command(async () => await GetUserData());
            EditOrder = new Command<Order>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
        }
        private async void OnAddItem(object obj)
        {
            if (Workers[0].Role == "admin" || Workers[0].Role == "root")
            {
                await Shell.Current.GoToAsync(nameof(NewItemPage));
            }
            else
            {
                alertService.ShowToast("У вас недостатно прав!!!", 1f);
            }
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                SetProperty(ref _selectedOrder, value);
                OnItemSelected(value);
            }
        }
        async void OnItemSelected(Order item)
        {
            if (item == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(DetailOrderPage)}?{nameof(DetailOrderViewModel.Id)}={item.Id}");
        }

        private async Task GetUserData()
        {
            try
            {
                alertService.ShowToast("Получение показаний...", 1f);
                OrdersList.Clear();
                EasyOrdersList.Clear();
                Workers.Clear();
                var list = await App.Database.GetUsersAsync();
                if (list.Count > 0)
                {
                    foreach (var user in list)
                    {
                        Workers.Add(user);
                    }
                    HttpClient _tokenclient = new HttpClient();
                    var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/order/finished/{Workers[0].Login}/{Workers[0].Token}");
                    var _jsonResults = JsonConvert.DeserializeObject<List<Request>>(_responseToken);
                    foreach (var x in _jsonResults)
                    {
                        x.RuState = RuState(x.State);
                        OrdersList.Add(x);
                    }
                    foreach (var x in _jsonResults)
                    {
                        EasyOrdersList.Add(new Order()
                        {
                            Id = x.Id,
                            Description = x.Description,
                            State = x.RuState,
                            Paper = CheckMaterial(x.Materials, "Пленка"),
                            Carton = CheckMaterial(x.Materials, "Картон"),
                            Poddon = CheckMaterial(x.Materials, "Поддоны"),
                            Address = x.Address,
                        });
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
                await alertService.ShowMessage(ex.Data.ToString(), ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }


        bool CheckMaterial(string _data, string _pattern)
        {
            bool res = false;
            var _separatedData = _data.Split(':', ';');
            foreach (var x in _separatedData)
            {
                if (x == _pattern)
                {
                    res = true;
                    break;
                }
            }
            return res;
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
                    res = "Вывезен";
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

        internal void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
