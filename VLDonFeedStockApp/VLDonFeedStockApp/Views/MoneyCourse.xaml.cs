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
    public partial class MoneyCourse : ContentPage
    {
        MoneyCourseViewModel _moneyCourseViewModel;
        public MoneyCourse()
        {
            InitializeComponent();
            BindingContext = _moneyCourseViewModel = new MoneyCourseViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _moneyCourseViewModel.OnAppearing();

        }
    }
}