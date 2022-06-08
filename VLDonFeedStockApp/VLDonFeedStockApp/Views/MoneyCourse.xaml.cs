using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class MoneyCourse : ContentPage
    {
        MoneyCourseViewModel _moneyCourseViewModel;
        public MoneyCourse()
        {
            InitializeComponent();
            BindingContext = _moneyCourseViewModel = new MoneyCourseViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _moneyCourseViewModel.OnAppearing();

        }

       
         

        private void ContrAgentEntry_Unfocused(object sender, FocusEventArgs e)
        {
            //if (ContrAgentPicker.Items != null)
            //{
            //    if (ContrAgentPicker.SelectedIndex != -1)
            //    {
            //        ContrAgentEntry.Text = ContrAgentPicker.Items[ContrAgentPicker.SelectedIndex];
            //    }
            //}
        }

        private void ContrAgentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ContrAgentEntry.Text = ContrAgentPicker.Items[ContrAgentPicker.SelectedIndex];
            //GetContrAgentsAsync();
        }

        public async void GetContrAgentsAsync(bool forceRefresh = false)
        {
            try
            {
                
                HttpClient _tokenClientPrice = new HttpClient();
                var _responseTokenPrice = await _tokenClientPrice.GetStringAsync($"{GlobalSettings.HostUrl}api/price/{ContrAgentEntry.Text}");
                var _jsonResultsPrice = JsonConvert.DeserializeObject<Prices>(_responseTokenPrice);
                _moneyCourseViewModel.Prices = _jsonResultsPrice;
                ContrAgentEntry.Text = string.Empty;
                //_moneyCourseViewModel.IsBusy = true;

            }
            catch (Exception ex)
            {
                await DisplayAlert("error", ex.Message, "OK");

            }

        }
    }
}