using System;
using System.ComponentModel;
using VLDonFeedStockApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;



namespace VLDonFeedStockApp.Views
{
    public partial class AboutPage : ContentPage
    {
        AboutViewModel _aboutViewModel;
        public AboutPage()
        {
            InitializeComponent();
            BindingContext = _aboutViewModel = new AboutViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _aboutViewModel.OnAppearing();
        }
    }
}