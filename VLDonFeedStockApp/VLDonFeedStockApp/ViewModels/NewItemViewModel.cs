using Newtonsoft.Json;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        IAlertService alertService;
        private Request _newOrder;
        private Workers _user;
        private string _carton;
        private string _plenka;
        private string _poddon;
        private string _cartonAmount;
        private string _plenkaAmount;
        private string _poddonAmount;
        public ObservableCollection<Workers> Users { get; }
        
        public Command LoadItemsCommand { get; }
        public Command CreateCommand { get; }
        public Command BackCommand { get; }
        public Workers User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }
        public Request NewOrder
        {
            get => _newOrder;
            set => SetProperty(ref _newOrder, value);
        }
        public NewItemViewModel()
        {
            alertService = DependencyService.Resolve<IAlertService>();
            Title = "Новая заявка";
            Users = new ObservableCollection<Workers>();
            NewOrder = new Request() 
            {
                State = "created",
            };
            LoadItemsCommand = new Command(async () => await GetUserData());
            CreateCommand = new Command(OnEditClicked);
            BackCommand = new Command(OnCancel);
        }
        private async void OnEditClicked(object obj)
        {
            try
            {

                Users.Clear();
                

                var list = await App.Database.GetUsersAsync();
                if (list.Count > 0)
                {
                    foreach (var user in list)
                    {
                        Users.Add(user);
                    }
                    User = Users[0];

                }
                
                if (User != null)
                {
                    NewOrder.RelatedOperator = User.Login;
                    NewOrder.RelatedOrganizationId = User.OrganizationId;
                    NewOrder.Organization = User.Organization;
                    if (NewOrder.Data != null)
                    {
                        NewOrder.Data = NewOrder.Data.Split(' ')[0].ToString();
                    }
                    else
                    {
                        NewOrder.Data = DateTime.Now.ToString().Split(' ')[0].ToString();
                    }
                    await CreateRequest(NewOrder);
                }
            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message, 1);
            }


        }

        public string Validator(string _data, string _amount)
        {
            if (String.IsNullOrEmpty(_data))
            {
                return ":;";
            }
            else
            {

                return $"{_data}:{_amount};";
            }
        }

        public string Plenka
        {
            get => _plenka;
            set => SetProperty(ref _plenka, value);
        }
        public string Poddon
        {
            get => _poddon;
            set => SetProperty(ref _poddon, value);
        }
        public string Carton
        {
            get => _carton;
            set => SetProperty(ref _carton, value);
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

        public string PlenkaAmount
        {
            get => _plenkaAmount;
            set => SetProperty(ref _plenkaAmount, value);
        }
        public string PoddonAmount
        {
            get => _poddonAmount;
            set => SetProperty(ref _poddonAmount, value);
        }
        public string CartonAmount
        {
            get => _cartonAmount;
            set => SetProperty(ref _cartonAmount, value);
        }

        public async Task<Request> CreateRequest(Request indications)
        {
            indications.Id = 0;
            indications.Address = User.Address;
            indications.Materials = $"{Validator(Plenka, PlenkaAmount)}{Validator(Carton, CartonAmount)}{Validator(Poddon, PoddonAmount)}";
            alertService.ShowToast("Идет обновление... Пожалуйста, подождите...", 1);
            IsBusy = true;
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{GlobalSettings.HostUrl}api/order/{User.Token}",
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
                HttpClient _tokenclientDiff = new HttpClient();
                var _responseTokenDiff = await _tokenclientDiff.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/store/{User.Organization}/{User.Address}");
                CrossFirebasePushNotification.Current.Subscribe($"{_responseTokenDiff}_{res.Id}");
                await alertService.ShowMessage("Заявка", "Успешно создано!!!");
                
                await Shell.Current.GoToAsync("..");
                return null;
            }
        }
    


        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        internal void OnAppearing()
        {
            IsBusy = true;
        }

    }
}
