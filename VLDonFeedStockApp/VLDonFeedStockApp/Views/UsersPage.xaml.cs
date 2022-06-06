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
    public partial class UsersPage : ContentPage
    {
        UsersViewModel _usersViewModel;
        public UsersPage()
        {
            InitializeComponent();
            BindingContext = _usersViewModel = new UsersViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _usersViewModel.OnAppearing();
        }
    }
}