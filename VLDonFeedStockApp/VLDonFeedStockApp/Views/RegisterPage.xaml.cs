using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VLDonFeedStockApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        RegisterViewModel _registerViewModel;
        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = _registerViewModel = new RegisterViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _registerViewModel.OnAppearing();
        }

        private async Task loginEntry_UnfocusedAsync(object sender, FocusEventArgs e)
        {
            try
            {
                if (loginEntry.Text != null)
                {
                    HttpClient client = new HttpClient();

                    var response = await client.GetAsync($"{GlobalSettings.HostUrl}api/auth/{loginEntry.Text}");
                    if (response != null)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {

                        }
                        else
                        {
                            loginEntry.Text = "Такой логин уже существует!!!";
                        }
                    }
                    if (loginEntry.Text.Length != 11)
                    {
                        loginEntry.Text = "Номер состоит из 11 цифр!!!";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void loginEntry_Unfocused(object sender, FocusEventArgs e)
        {

        }

        private void organizationEntry_Unfocused(object sender, FocusEventArgs e)
        {

        }

        private void CitiesPicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (CitiesPicker.Items != null)
            {
                if (CitiesPicker.SelectedIndex != -1)
                {
                    organizationEntry.Text = CitiesPicker.Items[CitiesPicker.SelectedIndex];
                }
            }
        }

        private async void CitiesPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                organizationEntry.Text = CitiesPicker.Items[CitiesPicker.SelectedIndex];
                var role = await DetectRole();
                if (role == "director")
                {
                    roleEntry.Text = "admin";
                    StoresPicker.IsEnabled = true;
                    GetOrganizationsAsync();
                }
                else
                {
                    roleEntry.Text = "employee";
                    StoresPicker.IsEnabled = false;
                    StoresPicker.SelectedIndex = -1;
                    storeEntry.Text = string.Empty;
                    
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("er", ex.Message, "ok");
            }           
        }

        public async Task<string> DetectRole()
        {
            HttpClient _tokenclientDiff = new HttpClient();
            var _responseTokenDiff = await _tokenclientDiff.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/role/{organizationEntry.Text}");
            await DisplayAlert("Регистрация",$"Вы будете зарегистрированы как: {SetRole(_responseTokenDiff)}" , "ОК");
            return _responseTokenDiff;
        }

        public string SetRole(string _data)
        {
            switch (_data)
            {
                case "director":
                    _data = "Директор";
                    break;
                case "root":
                    _data = "Администратор";
                    break;
                default:
                    _data = "КонтрАгент";
                    break;
            }
            return _data;
        }


        private void StoresPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            storeEntry.Text = StoresPicker.Items[StoresPicker.SelectedIndex];
        }

        private void StoresPicker_Unfocused(object sender, FocusEventArgs e)
        {

        }

        public async void GetOrganizationsAsync(bool forceRefresh = false)
        {
            try
            {
                StoresPicker.Items.Clear();
                HttpClient _tokenclient = new HttpClient();
                var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/auth/stores/{organizationEntry.Text}");
                var _jsonResults = JsonConvert.DeserializeObject<List<Stores>>(_responseToken);
                foreach (var store in _jsonResults)
                {
                    StoresPicker.Items.Add(store.Address);
                }
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("error", ex.Message,"OK");
               
            }

        }

        private async void loginEntry_Unfocused_1(object sender, FocusEventArgs e)
        {
            try
            {
                if (loginEntry.Text != null)
                {
                    if (loginEntry.Text.Length != 11)
                    {
                        loginEntry.PlaceholderColor = Color.Red;
                        loginEntry.Placeholder = "Номер состоит из 11 цифр!!!";
                        loginEntry.Text = string.Empty;
                    }
                    else
                    {
                        HttpClient client = new HttpClient();

                        var response = await client.GetAsync($"{GlobalSettings.HostUrl}api/auth/{loginEntry.Text}");
                        if (response != null)
                        {
                            if (response.StatusCode != HttpStatusCode.OK)
                            {

                            }
                            else
                            {
                                loginEntry.PlaceholderColor = Color.Red;
                                loginEntry.Placeholder = "Такой логин уже существует!!!";
                                loginEntry.Text = string.Empty;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void RolesPicker_Unfocused(object sender, FocusEventArgs e)
        {

        }
    }
}