﻿using Newtonsoft.Json;
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
    public class HistoryViewModel : BaseViewModel
    {
        public ObservableCollection<Order> StoresList { get; }
        public ObservableCollection<Workers> WorkersList { get; }

        private IAlertService alertService;

        public Command LoadStoresCommand { get; }
        public HistoryViewModel()
        {
            Title = "История";
            StoresList = new ObservableCollection<Order>();
            WorkersList = new ObservableCollection<Workers>();
            alertService = DependencyService.Resolve<IAlertService>();
            LoadStoresCommand = new Command(async () => await GetUserData());
        }

        private async Task GetUserData()
        {
            try
            {
                alertService.ShowToast("Получение показаний...", 1f);

                WorkersList.Clear();
                StoresList.Clear();
                var list = await App.Database.GetUsersAsync();
                if (list.Count > 0)
                {
                    foreach (var user in list)
                    {
                        WorkersList.Add(user);
                    }
                    if (WorkersList[0].Role == "root" || WorkersList[0].Role == "employee")
                    {
                        HttpClient _tokenclient = new HttpClient();
                        _tokenclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WorkersList[0].Token);
                        var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/order/{WorkersList[0].Login}/{WorkersList[0].UserToken}/history");
                        var _jsonResults = JsonConvert.DeserializeObject<List<Order>>(_responseToken);
                        foreach (var x in _jsonResults)
                        {
                            StoresList.Add(x);
                        }

                        alertService.ShowToast("Данные получены...", 1f);
                    }
                    else
                    {
                        await alertService.ShowMessage("История", "История доступна только контрагентам");
                        await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                    }
                }
                else
                {
                    alertService.ShowToast("Авторизируйтесь...", 1f);
                    await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                }
            }
            catch (Exception ex)
            {
                if (WorkersList[0].IsAccepted == false)
                {
                    await alertService.ShowMessage("Регистрация", "Ваша учетная запись не подтверждена, обратитесь к администратору!!!");
                }
                else
                {
                    await alertService.ShowMessage("Сервер", "Сервер временно недоступен... Приносим извинения... :с");
                }

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
