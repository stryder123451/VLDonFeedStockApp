using System;
using System.Collections.Generic;
using System.ComponentModel;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VLDonFeedStockApp.Views
{
    public partial class NewItemPage : ContentPage
    {
        NewItemViewModel _NewItemViewModel;
        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = _NewItemViewModel = new NewItemViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _NewItemViewModel.OnAppearing();
        }
    }
}