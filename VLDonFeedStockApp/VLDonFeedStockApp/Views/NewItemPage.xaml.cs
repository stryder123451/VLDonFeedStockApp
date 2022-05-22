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
                PlenkaEntryHidden.IsEnabled = true;
                PlenkaEntryHidden.Text = "Пленка";
            }
            else
            {
                PlenkaEntryHidden.IsEnabled = false;
                PlenkaEntryHidden.Text = string.Empty;
            }
            //CheckForAmounts();
        }

        private void isCarton_Toggled(object sender, ToggledEventArgs e)
        {
            if (isCarton.IsToggled)
            {
                CartonEntryHidden.IsEnabled = true;
                CartonEntryHidden.Text = "Картон";
            }
            else
            {
                CartonEntryHidden.IsEnabled = false;
                CartonEntryHidden.Text = string.Empty;
            }
            //CheckForAmounts();
        }

        private void isPoddon_Toggled(object sender, ToggledEventArgs e)
        {
            if (isPoddon.IsToggled)
            {
                PoddonEntryHidden.IsEnabled = true;
                PoddonEntryHidden.Text = "Поддоны";
            }
            else
            {
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
