﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public ObservableCollection<Models.Organizations> Organizations { get; }
        public ObservableCollection<Stores> Stores { get; }
        public Command RegisterCommand { get; }
        public Command BackCommand { get; }
        public Command Return { get; }
        public Command LoadItemsCommand { get; }
        private string _login;
        private string _password;
        private string _role;
        private string _organization;
        private string _name;
        private string _address;
        public IAlertService alertService;
        public RegisterViewModel()
        {
            Organizations = new ObservableCollection<Models.Organizations>();
            Stores = new ObservableCollection<Stores>();
            LoadItemsCommand = new Command(async () => await GetRegisterData());
            RegisterCommand = new Command(OnRegisterClicked);
            alertService = DependencyService.Resolve<IAlertService>();
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            BackCommand = new Command(OnCancel);
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        public string Organization
        {
            get => _organization;
            set => SetProperty(ref _organization, value);
        }


        private async Task GetRegisterData()
        {
            IsBusy = true;
            try
            {
                Organizations.Clear();
                IEnumerable<Models.Organizations> organizationsItems = await GetOrganizationsAsync(true);

                foreach (var item in organizationsItems)
                {
                    Organizations.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnRegisterClicked(object obj)
        {
            if (ValidateSave())
            {
                Workers user = new Workers()
                {
                    Name = Name,
                    Login = Login,
                    Password = Password,
                    Organization = Organization,
                    Address = Address,
                };
                await AddNewUser(user);

                // await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(Login)
                &&!String.IsNullOrWhiteSpace(Name)
               && !String.IsNullOrWhiteSpace(Password)
               && !String.IsNullOrWhiteSpace(Organization);
        }

        public async Task<Workers> AddNewUser(Workers user)
        {
            user.Role = Role;
            if (String.IsNullOrEmpty(Address))
            {
                Address = "test";
            }
            RegisterModel _registerData = new RegisterModel()
            {
                Address = Address,
                 Login = Login,
                  Organization = Organization,
                   OrganizationId = user.OrganizationId,
                    Role = user.Role,
                     Password = Password,
                      WorkerName  = Name

            };
            
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{GlobalSettings.HostUrl}api/MobileAccount/loginreg",
                new StringContent(
                   System.Text.Json.JsonSerializer.Serialize(_registerData),
            Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                await alertService.ShowMessage("Регистрация", "Проверьте корректность введенных данных!!!");
                return null;
            }
            else
            {
                await alertService.ShowMessage("Регистрация", "Успешно!!!");
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                return null;

            }
        }

        public async Task<IEnumerable<Models.Organizations>> GetOrganizationsAsync(bool forceRefresh = false)
        {
            try
            {
    
                HttpClient _tokenclient = new HttpClient();
                var url = $"{GlobalSettings.HostUrl}api/auth/organizations";
                var _responseToken = await _tokenclient.GetStringAsync(url);
                var _jsonResults = JsonConvert.DeserializeObject<List<Organizations>>(_responseToken);
                return (IEnumerable<Models.Organizations>)await Task.FromResult(_jsonResults.Distinct());
            }
            catch (Exception ex)
            {
                await alertService.ShowMessage("error", ex.Message);
                return null;
            }
            
        }
    }
}
