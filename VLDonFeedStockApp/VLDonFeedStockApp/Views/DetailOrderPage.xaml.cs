using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.ViewModels;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VLDonFeedStockApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailOrderPage : ContentPage
    {
        DetailOrderViewModel _detailOrderViewModel;
        public DetailOrderPage()
        {
            InitializeComponent();
            BindingContext = _detailOrderViewModel = new DetailOrderViewModel();
            
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _detailOrderViewModel.OnAppear();
            try
            {
                await CheckForEditAsync();
            }
            catch (Exception ex)
            {
                
            }
        }

        async Task CheckForEditAsync()
        {
            var list = await App.Database.GetUsersAsync();
            var res = string.Empty;
            if (list.Count > 0)
            {
               res = list[0].Role;
            }
            if (res != null && res.Length == "employee".Length)
            {
                isPlenka.IsEnabled = false;
                isCarton.IsEnabled = false;
                isPoddon.IsEnabled = false;
            }
        }

        private void isPlenka_Toggled(object sender, ToggledEventArgs e)
        {
            //if (_detailOrderViewModel.User.Role == "employeе" || _detailOrderViewModel.User.Role == "root")
            //{
            //    if (_detailOrderViewModel.IsPlenka)
            //    {

            //    }
            //}
            //CheckForAmounts();
            if (isPlenka.IsToggled)
            {
                PlenkaEntryHidden.IsEnabled = true;
                PlenkaEntryHidden.Text = "Пленка:;";
            }
            else
            {
                PlenkaEntryHidden.IsEnabled = false;
                PlenkaEntryHidden.Text = string.Empty;
            }
        }

        private void isCarton_Toggled(object sender, ToggledEventArgs e)
        {
            //if (_detailOrderViewModel.User.Role == "employeе" || _detailOrderViewModel.User.Role == "root")
            //{
            //    if (_detailOrderViewModel.IsCarton)
            //    {

            //    }
            //}
            //CheckForAmounts();
            if (isCarton.IsToggled)
            {
                CartonEntryHidden.IsEnabled = true;
                CartonEntryHidden.Text = "Картон:;";
            }
            else
            {
                CartonEntryHidden.IsEnabled = false;
                CartonEntryHidden.Text = string.Empty;
            }
        }

        private void isPoddon_Toggled(object sender, ToggledEventArgs e)
        {
            //if (_detailOrderViewModel.User.Role == "employeе" || _detailOrderViewModel.User.Role == "root")
            //{
            //    if (_detailOrderViewModel.IsPoddon)
            //    {

            //    }
            //}
            //CheckForAmounts();
            
            if (isPoddon.IsToggled)
            {
                PoddonEntryHidden.IsEnabled = true;
                PoddonEntryHidden.Text = "Поддоны:;";
            }
            else
            {
                PoddonEntryHidden.IsEnabled = false;
                PoddonEntryHidden.Text = string.Empty;
            }
        }

        private void PlenkaEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var res = AmountValidator(PlenkaEntryHidden.Text, PlenkaEntry.Text);
            if (res != null)
            {
                PlenkaEntryHidden.Text = res;
            }
        }

        private void CartonEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var res = AmountValidator(CartonEntryHidden.Text, CartonEntry.Text);
            if (res != null)
            {
                CartonEntryHidden.Text = res;
            }
        }

        private void PoddonEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var res = AmountValidator(PoddonEntryHidden.Text, PoddonEntry.Text);
            if (res != null)
            {
                PoddonEntryHidden.Text = res;
            }
        }

        private string AmountValidator (string _hiddenMaterialText, string _materialText)
        {
            //var mass = _hiddenMaterialText.Split(':');
            //if (mass.Length < 2)
            //{

            //}
            if (!String.IsNullOrEmpty(_materialText))
            {
                if (!String.IsNullOrEmpty(_hiddenMaterialText))
                {
                    if (_hiddenMaterialText.Split(':').Length > 1)
                    {
                        var res = _hiddenMaterialText.Split(':')[0];
                        return _hiddenMaterialText = $"{res}:{_materialText};";
                    }
                    return _hiddenMaterialText = $"{_hiddenMaterialText}:{_materialText};";

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void CheckForAmounts()
        {
            AmountValidator(PlenkaEntryHidden.Text, PlenkaEntry.Text);
            AmountValidator(CartonEntryHidden.Text, CartonEntry.Text);
            AmountValidator(PoddonEntryHidden.Text, PoddonEntry.Text);
        }

        private void PlenkaEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            PriceEntry.Text = TotalPrice().ToString();
        }

        private void CartonEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            PriceEntry.Text = TotalPrice().ToString();
        }

        private void PoddonEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            PriceEntry.Text = TotalPrice().ToString();
        }

        float TotalPrice()
        {
            return (TextValidator(PlenkaEntry.Text) * TextValidator(_detailOrderViewModel.Prices.Plenka) +
            (TextValidator(PoddonEntry.Text) * TextValidator(_detailOrderViewModel.Prices.Poddon) +
            (TextValidator(CartonEntry.Text) * TextValidator(_detailOrderViewModel.Prices.Carton))));

        }

        float TextValidator(string _data)
        {
            if (string.IsNullOrEmpty(_data))
            {
                return 0;
            }
            else
            {
                return float.Parse(_data);
            }
        }

        private async void DisplayActionAddPhoto()
        {
            //var res = await DisplayActionSheet("Изменить фото...", "Отмена", null,"Добавить фото Поддонов", "Добавить фото Пленки", "Добавить фото Картона");
            //switch (res)
            //{
            //    case "Добавить фото Поддонов":
            //        _detailOrderViewModel.TakePhotoPoddon();
            //        break;
            //    case "Добавить фото Пленки":
            //        _detailOrderViewModel.TakePhotoPlenka();
            //        break;
            //    case "Добавить фото Картона":
            //        _detailOrderViewModel.TakePhotoCarton();
            //        break;
            //}
        }

        private async void DisplayActionCreatePhoto()
        {
            //var res = await DisplayActionSheet("Сделать фото...", "Отмена", null, "Сделать фото Поддонов", "Сделать фото Пленки", "Сделать фото Картона");
            //switch (res)
            //{
            //    case "Сделать фото Поддонов":
            //        _detailOrderViewModel.CreatePhotoPoddon();
            //        break;
            //    case "Сделать фото Пленки":
            //        _detailOrderViewModel.CreatePhotoPlenka();
            //        break;
            //    case "Сделать фото Картона":
            //        _detailOrderViewModel.CreatePhotoCarton();
            //        break;
            //}
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            DisplayActionAddPhoto();
        }

        private void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            DisplayActionCreatePhoto();
        }
    }
}