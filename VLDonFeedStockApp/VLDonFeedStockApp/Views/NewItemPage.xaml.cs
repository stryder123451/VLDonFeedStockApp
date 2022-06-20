using System;
using System.Collections.Generic;
using System.ComponentModel;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VLDonFeedStockApp.Views
{
    public partial class NewItemPage : ContentPage
    {
        NewItemViewModel _NewItemViewModel;
        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = _NewItemViewModel = new NewItemViewModel();
            DataPicker.Date = DateTime.Now;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _NewItemViewModel.OnAppearing();
           
        }

        private void isPlenka_Toggled(object sender, ToggledEventArgs e)
        {
            if (isPlenka.IsToggled)
            {
                PlenkaEntry.IsEnabled = true;
                PlenkaEntryHidden.IsEnabled = true;
                PlenkaEntryHidden.Text = "Пленка";
            }
            else
            {
                PlenkaEntry.IsEnabled = false;
                PlenkaEntryHidden.IsEnabled = false;
                PlenkaEntryHidden.Text = string.Empty;
            }
            //CheckForAmounts();
        }

        private void isCarton_Toggled(object sender, ToggledEventArgs e)
        {
            if (isCarton.IsToggled)
            {
                CartonEntry.IsEnabled = true;
                CartonEntryHidden.IsEnabled = true;
                CartonEntryHidden.Text = "Картон";
            }
            else
            {
                CartonEntry.IsEnabled = false;
                CartonEntryHidden.IsEnabled = false;
                CartonEntryHidden.Text = string.Empty;
            }
            //CheckForAmounts();
        }

        private void isPoddon_Toggled(object sender, ToggledEventArgs e)
        {
            if (isPoddon.IsToggled)
            {
                PoddonEntry.IsEnabled = true;
                PoddonEntryHidden.IsEnabled = true;
                PoddonEntryHidden.Text = "Поддоны";
            }
            else
            {
                PoddonEntry.IsEnabled = false;
                PoddonEntryHidden.IsEnabled = false;
                PoddonEntryHidden.Text = string.Empty;
            }
            //CheckForAmounts();
        }

        private void PlenkaEntry_Unfocused(object sender, FocusEventArgs e)
        {
            AmountValidator(PlenkaEntryHidden.Text, PlenkaEntry.Text);
        }

        private void CartonEntry_Unfocused(object sender, FocusEventArgs e)
        {
            AmountValidator(CartonEntryHidden.Text, CartonEntry.Text);
        }

        private void PoddonEntry_Unfocused(object sender, FocusEventArgs e)
        {
            AmountValidator(PoddonEntryHidden.Text, PoddonEntry.Text);
        }

        private void AmountValidator(string _hiddenMaterialText, string _materialText)
        {
            var mass = _hiddenMaterialText.Split(':');
            if (mass.Length < 2)
            {
                if (!String.IsNullOrEmpty(_materialText))
                {
                    if (!String.IsNullOrEmpty(_hiddenMaterialText))
                    {
                        _hiddenMaterialText += $":{_materialText};";
                    }
                }
            }
        }

        private void CheckForAmounts()
        {
            AmountValidator(PlenkaEntryHidden.Text, PlenkaEntry.Text);
            AmountValidator(CartonEntryHidden.Text, CartonEntry.Text);
            AmountValidator(PoddonEntryHidden.Text, PoddonEntry.Text);
        }
    }
}
