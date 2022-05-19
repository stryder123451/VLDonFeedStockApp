using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VLDonFeedStockApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatedOrdersPage : ContentPage
    {
        CreatedOrdersViewModel _createdOrdersViewModel;
        public CreatedOrdersPage()
        {
            InitializeComponent();
            BindingContext = _createdOrdersViewModel = new CreatedOrdersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _createdOrdersViewModel.OnAppearing();
        }

        private async void OrderItems_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            
            //Request request = e.Item as Request;
            //if (request != null)
            //{
            //    await DisplayAlert("test",request.RuState,"OK");
            //}
        }

        private async void EditBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                var menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    var order = menuItem.CommandParameter as Request;
                    await DisplayAlert("test", order.RuState, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("test", ex.Message, "OK");
            }
        }
    }
}