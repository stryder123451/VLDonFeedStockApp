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
    public partial class MaterialShow : ContentPage
    {
        ShowMaterialViewModel _showMaterialViewModel; 
        public MaterialShow()
        {
            InitializeComponent();
            BindingContext = _showMaterialViewModel = new ShowMaterialViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _showMaterialViewModel.OnAppearing();
        }

    }
}