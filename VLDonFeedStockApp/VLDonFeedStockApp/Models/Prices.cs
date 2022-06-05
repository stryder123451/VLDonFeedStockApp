using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class Prices
    {
        public int Id { get; set; }
        public string Carton { get; set; }
        public string Poddon { get; set; }
        public string Plenka { get; set; }
        public string Version { get; set; }
    }
}
