using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Services
{
    public interface IFileService
    {
        void CreateFile(string v);
        string GetRootPath();
    }
}
