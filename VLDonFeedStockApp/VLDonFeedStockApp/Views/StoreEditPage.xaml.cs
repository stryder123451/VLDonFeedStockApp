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
    public partial class StoreEditPage : ContentPage
    {
        StoreEditViewModel _storeEditViewModel;
        public StoreEditPage()
        {
            InitializeComponent();
            BindingContext = _storeEditViewModel = new StoreEditViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _storeEditViewModel.OnAppearing();
        }

        private void AgentsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AgentsPicker.SelectedIndex != -1)
            {
                ContrAgentEntry.Text = AgentsPicker.Items[AgentsPicker.SelectedIndex];
            }
        }

        private void AgentsPicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (AgentsPicker.Items != null)
            {
                if (AgentsPicker.SelectedIndex != -1)
                {
                    ContrAgentEntry.Text = AgentsPicker.Items[AgentsPicker.SelectedIndex];
                }
            }
        }
    }
}