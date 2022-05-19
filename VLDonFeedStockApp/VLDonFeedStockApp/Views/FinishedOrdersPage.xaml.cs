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
    public partial class FinishedOrdersPage : ContentPage
    {
        FinishedOrdersViewModel _finishedOrdersViewModel;
        public FinishedOrdersPage()
        {
            InitializeComponent();
            BindingContext = _finishedOrdersViewModel = new FinishedOrdersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _finishedOrdersViewModel.OnAppearing();
        }
    }
}