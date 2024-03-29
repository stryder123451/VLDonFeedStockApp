﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VLDonFeedStockApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoresPage : ContentPage
    {
        StoresViewModel _storesViewModel;
        
        
        public StoresPage()
        {  
            InitializeComponent();
            BindingContext = _storesViewModel = new StoresViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _storesViewModel.OnAppearing();
        }
    }
}