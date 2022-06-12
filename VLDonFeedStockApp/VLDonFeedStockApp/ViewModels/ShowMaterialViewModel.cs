using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    [QueryProperty(nameof(Material), nameof(Material))]
    [QueryProperty(nameof(Address), nameof(Address))]
    [QueryProperty(nameof(Id), nameof(Id))]
    public class ShowMaterialViewModel : BaseViewModel
    {
        public Command LoadItemsCommand { get; }
        private string _material;
        private string _id;
        private string _address;
        private ImageSource _materialSource;

        public ImageSource MaterialSource
        {
            get => _materialSource;
            set => SetProperty(ref _materialSource, value);
        }
        public Command BackCommand { get; }
        
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value); 
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string Material
        {
            get => _material;
            set => SetProperty(ref _material, value);
        }

        
        public ShowMaterialViewModel()
        {
            LoadItemsCommand = new Command(async () => GetPhoto());
            BackCommand = new Command(OnCancel);
        }

        private void OnCancel(object obj)
        {
            BackToOrder();
        }

        private async void BackToOrder()
        {
            await Shell.Current.GoToAsync($"{nameof(DetailOrderPage)}?{nameof(DetailOrderViewModel.Id)}={Id}&{nameof(DetailOrderViewModel.Address)}={Address}");
        }

        private void GetPhoto()
        {
            try
            {
                MaterialSource = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Id}_{Address}_{Material}.png";
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }

        }

        internal void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
