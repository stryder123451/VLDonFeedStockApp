using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Services
{
    public interface IAlertService
    {
        Task ShowMessage(string header, string message);
        void ShowToast(string message, float duration);
    }
}
