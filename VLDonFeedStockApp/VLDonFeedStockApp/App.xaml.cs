using System;
using System.IO;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.ViewModels;
using VLDonFeedStockApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
[assembly: ExportFont("deusex.otf", Alias = "DeusEx")]
namespace VLDonFeedStockApp
{
    public partial class App : Application
    {
        
        private static Database _database;
        public static Database Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Workers.db3"));
                }
                return _database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
           
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
