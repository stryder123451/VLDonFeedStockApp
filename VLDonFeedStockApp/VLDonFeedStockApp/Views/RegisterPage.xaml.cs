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

        private void CitiesPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            organizationEntry.Text = CitiesPicker.Items[CitiesPicker.SelectedIndex];
            GetOrganizationsAsync();
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

    }
}