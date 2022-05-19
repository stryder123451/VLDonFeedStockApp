using Acr.UserDialogs;
using System;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AlertService))]
namespace VLDonFeedStockApp.Models
{
    
    public class AlertService : IAlertService
    {
        public async Task ShowMessage(string header, string message)
        {
            var config = new AlertConfig()
            {
                Title = header,
                Message = message,
                OkText = "OK",
            };
            await UserDialogs.Instance.AlertAsync(config);
        }

        public void ShowToast(string message, float duration)
        {
            var config = new ToastConfig(message)
            {
                Message = message,
                Position = ToastPosition.Bottom,
                Duration = TimeSpan.FromSeconds((double)new decimal(duration)),

                //Icon = "ready.png"
            };
            UserDialogs.Instance.Toast(config);
        }
    }
}
