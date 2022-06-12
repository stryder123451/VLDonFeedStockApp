using Android.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Droid;
using VLDonFeedStockApp.Services;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace VLDonFeedStockApp.Droid
{
    public class FileService : IFileService
    {
        public string GetRootPath()
        {
            return Application.Context.GetExternalFilesDir(null).ToString();
        }

        public void CreateFile(string file)
        {
            var _destination = Path.Combine(GetRootPath(), file);
        }
    }
}
