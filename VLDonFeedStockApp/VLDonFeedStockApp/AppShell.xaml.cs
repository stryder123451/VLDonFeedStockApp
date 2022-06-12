using System;
using System.Collections.Generic;
using VLDonFeedStockApp.ViewModels;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;

namespace VLDonFeedStockApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        AboutViewModel _aboutViewModel;
        public bool AdminButton;
        public AppShell()
        {
            BindingContext = _aboutViewModel = new AboutViewModel();
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(CreatedOrdersPage), typeof(CreatedOrdersPage));
            Routing.RegisterRoute(nameof(ActualOrdersPage), typeof(ActualOrdersPage));
            Routing.RegisterRoute(nameof(DetailOrderPage), typeof(DetailOrderPage));
            Routing.RegisterRoute(nameof(WeightedOrdersPage), typeof(WeightedOrdersPage));
            Routing.RegisterRoute(nameof(FinishedOrdersPage), typeof(FinishedOrdersPage));
            Routing.RegisterRoute(nameof(AboutUs), typeof(AboutUs));
            Routing.RegisterRoute(nameof(StoresPage), typeof(StoresPage));
            Routing.RegisterRoute(nameof(StoreEditPage), typeof(StoreEditPage));
            Routing.RegisterRoute(nameof(MoneyCourse), typeof(MoneyCourse));
            Routing.RegisterRoute(nameof(UsersPage), typeof(UsersPage));
            Routing.RegisterRoute(nameof(ContrAgentsPrices), typeof(ContrAgentsPrices));
            Routing.RegisterRoute(nameof(MaterialShow), typeof(MaterialShow));
            
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            
            await Shell.Current.GoToAsync("//LoginPage");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _aboutViewModel.OnAppearing();
        }

    }
}
