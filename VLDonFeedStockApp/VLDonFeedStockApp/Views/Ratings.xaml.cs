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
    public partial class Ratings : ContentPage
    {
        RatingsViewModel _ratingsViewModel;
        public Ratings()
        {
            InitializeComponent();
            BindingContext = _ratingsViewModel = new RatingsViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ratingsViewModel.OnAppearing();
        }
    }
}