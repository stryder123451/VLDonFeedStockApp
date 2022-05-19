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
    public partial class ActualOrdersPage : ContentPage
    {
        ActualOrdersViewModel _actualOrdersViewModel;
        public ActualOrdersPage()
        {
            InitializeComponent();
            BindingContext = _actualOrdersViewModel = new ActualOrdersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _actualOrdersViewModel.OnAppearing();
        }
    }
}