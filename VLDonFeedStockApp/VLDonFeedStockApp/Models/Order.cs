using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public bool Paper { get; set; }
        public bool Carton { get; set; }
        public bool Poddon { get; set; }
        public string Address { get; set; }
    }
}
