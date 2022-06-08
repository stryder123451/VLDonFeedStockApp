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
    public partial class ContrAgentsPrices : ContentPage
    {
        ContrAgentsPricesViewModel _contrAgentsPricesViewModel;
        public ContrAgentsPrices()
        {
            InitializeComponent();
            BindingContext = _contrAgentsPricesViewModel = new ContrAgentsPricesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _contrAgentsPricesViewModel.OnAppearing();

        }
    }
}