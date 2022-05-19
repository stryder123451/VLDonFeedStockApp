using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public static class GlobalSettings
    {
        public static string HostUrlSSL { get; set; } = "https://192.168.0.107:44301/";
        public static string HostUrl { get; set; } = "http://192.168.0.107:8081/";

    }
}
