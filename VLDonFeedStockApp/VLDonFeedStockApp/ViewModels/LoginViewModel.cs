using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
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
    public class LoginViewModel : BaseViewModel
    {
        public IAlertService alertService;
        public Command LoginCommand { get; }
        public Command RegisterCommand { get; }
        private string _username;
        private string _password;
        private string _token;
        private bool _isLoggedIn;
        private bool _isLogging;
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

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }
        public bool IsLogging
        {
            get => _isLogging;
            set => SetProperty(ref _isLogging, value);
        }
        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            RegisterCommand = new Command(OnRegisterClicked);
            alertService = DependencyService.Resolve<IAlertService>();
            IsLogging = true;
            IsLoggedIn = false;
            
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
                //MessagingCenter.Unsubscribe<LoginViewModel>(this, "admin");
                IsLoggedIn = true;
                IsLogging = false;
                if (!String.IsNullOrWhiteSpace(Username) && !String.IsNullOrWhiteSpace(Password))
                {
                    
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
                                    LoginModel _loginModel = new LoginModel()
                                    {
                                         Login = Username,
                                         Password = Password,
                                    };


                                    alertService.ShowToast("Аутентификация...Пожалуйста, подождите...", 2f);
                                    HttpClient _tokenclient = new HttpClient();
                                    var _responseTokenJWT = await _tokenclient.PostAsync($"{GlobalSettings.HostUrl}api/MobileAccount/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(_loginModel),
                                      Encoding.UTF8, "application/json"));
                                    //
                            
                                    //
                                    if (_responseTokenJWT.IsSuccessStatusCode)
                                    {
                                        var _jwtToken = JsonConvert.DeserializeObject<ResponseModel>(_responseTokenJWT.Content.ReadAsStringAsync().Result).Token;

                                        //var _jsonResults = JsonConvert.DeserializeObject<Workers>(_responseToken);
                                        //await alertService.ShowMessage("Аутентификация", $"Здравствуйте, {_jsonResults.FullName}!!!");
                                        HttpClient _tokenclientLogin = new HttpClient();
                                        _tokenclientLogin.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                                        var _responseTokenLogin = await _tokenclientLogin.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/{Username}/{Password}/{_token}");
                                        var _jsonResults = JsonConvert.DeserializeObject<Workers>(_responseTokenLogin);

                                        //
                                        //HttpClient _tokenClientPrice = new HttpClient();
                                        //_tokenClientPrice.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                                        //var _responseTokenPrice = await _tokenClientPrice.GetStringAsync($"{GlobalSettings.HostUrl}api/price/ИП Леза Александр Владимирович");
                                        //var _jsonResultsPrice = JsonConvert.DeserializeObject<Prices>(_responseTokenPrice);
                                        //
                                        await App.Database.Login(_jsonResults, _jwtToken,GlobalSettings.Version);
                                        IsLoggedIn = false;
                                        IsLogging = true;
                                        //if (_jsonResults.Role == "root")
                                        //{
                                        //    MessagingCenter.Send<LoginViewModel>(this, "root");
                                        //}
                                        await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                                    }
                                    // await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                                }
                                catch (Exception ex)
                                {
                                    await alertService.ShowMessage("Аутентификация", ex.Message);
                                    IsLoggedIn = false;
                                    IsLogging = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        _token = null;
                        alertService.ShowToast("Введите корректные данные...", 2f);
                        IsLoggedIn = false;
                        IsLogging = true;
                    }


                }
                else
                {
                    alertService.ShowToast("Введите корректные данные...", 2f);
                    IsLoggedIn = false;
                    IsLogging = true;
                }
            }
            catch (Exception ex)
            {
                await alertService.ShowMessage("Аутентификация", ex.Message);
                IsLoggedIn = false;
                IsLogging = true;
            }
        }
    }
}
