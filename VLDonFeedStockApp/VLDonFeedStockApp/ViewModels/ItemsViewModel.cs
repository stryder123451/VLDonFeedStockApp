using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        internal void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
