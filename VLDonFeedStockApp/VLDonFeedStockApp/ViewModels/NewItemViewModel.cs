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
                    NewOrder.Data = NewOrder.Data.Split(' ')[0].ToString();
                    
                    await CreateRequest(NewOrder);
                }
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

        public async Task<Request> CreateRequest(Request indications)
        {
            alertService.ShowToast("Идет обновление... Пожалуйста, подождите...", 1);
            IsBusy = true;
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{GlobalSettings.HostUrl}api/order",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(indications),
            Encoding.UTF8, "application/json"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                alertService.ShowToast("Ошибка при обновлении... Попробуйте позже...", 1);
                return null;
            }
            else
            {
                await alertService.ShowMessage("Заявка","Успешно создано!!!");
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
