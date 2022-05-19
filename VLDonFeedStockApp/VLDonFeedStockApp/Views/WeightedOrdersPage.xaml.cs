using System;
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
    public partial class WeightedOrdersPage : ContentPage
    {
        WeightedOrdersViewModel _weightedOrdersViewModel;
        public WeightedOrdersPage()
        {
            InitializeComponent();
            BindingContext = _weightedOrdersViewModel = new WeightedOrdersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _weightedOrdersViewModel.OnAppearing();
        }
    }
}