using System;
using System.Collections.Generic;
using VLDonFeedStockApp.ViewModels;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;

namespace VLDonFeedStockApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(CreatedOrdersPage), typeof(CreatedOrdersPage));
            Routing.RegisterRoute(nameof(ActualOrdersPage), typeof(ActualOrdersPage));
            Routing.RegisterRoute(nameof(DetailOrderPage), typeof(DetailOrderPage));
            Routing.RegisterRoute(nameof(WeightedOrdersPage), typeof(WeightedOrdersPage));
            Routing.RegisterRoute(nameof(FinishedOrdersPage), typeof(FinishedOrdersPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
