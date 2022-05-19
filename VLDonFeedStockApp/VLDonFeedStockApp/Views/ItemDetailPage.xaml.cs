using System.ComponentModel;
using VLDonFeedStockApp.ViewModels;
using Xamarin.Forms;

namespace VLDonFeedStockApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}