using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public class LoginViewModel : BaseViewModel
    {
        public IAlertService alertService;
        public Command LoginCommand { get; }
        public Command RegisterCommand { get; }
        private string _username;
        private string _password;
        private string _token;
        public bool IsLogging { get; set; }
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            RegisterCommand = new Command(OnRegisterClicked);
            alertService = DependencyService.Resolve<IAlertService>();
        }

        private async void OnLoginClicked(object obj)
        {
            await Login();
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }
        private async void OnRegisterClicked(object obj)
        {
            //await alertService.ShowMessage("Hello", "Test");
            alertService.ShowToast("Переходим...",1);
            await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
        }

        private async Task Login()
        {
            try
            {

                if (!String.IsNullOrWhiteSpace(Username) && !String.IsNullOrWhiteSpace(Password))
                {
                    IsLogging = false;
                    alertService.ShowToast("Идентификация...Пожалуйста, подождите...", 2f);
                    HttpClient client = new HttpClient();
                    var response = await client.GetAsync($"{GlobalSettings.HostUrl}api/auth/{Username}/{Password}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var _result = await client.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/{Username}/{Password}");
                        _token = _result;
                        if (_token != null)
                        {
                            if (!String.IsNullOrWhiteSpace(Username) && !String.IsNullOrWhiteSpace(Password))
                            {
                                try
                                {
                                    alertService.ShowToast("Аутентификация...Пожалуйста, подождите...", 2f);
                                    HttpClient _tokenclient = new HttpClient();
                                    var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/{Username}/{Password}/{_token}");
                                    var _jsonResults = JsonConvert.DeserializeObject<Workers>(_responseToken);
                                    //await alertService.ShowMessage("Аутентификация", $"Здравствуйте, {_jsonResults.FullName}!!!");
                                    IsLogging = true;
                                    await App.Database.Login(_jsonResults);
                                    await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                                    // await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                                }
                                catch (Exception ex)
                                {
                                    await alertService.ShowMessage("Аутентификация", ex.Message);
                                    IsLogging = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        _token = null;
                        alertService.ShowToast("Введите корректные данные...", 2f);
                        IsLogging = true;
                    }


                }
                else
                {
                    alertService.ShowToast("Введите корректные данные...", 2f);
                }
            }
            catch (Exception ex)
            {
                await alertService.ShowMessage("Аутентификация", ex.Message);
            }
        }
    }
}
