using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental
{
    // Represents an order for a customer in the order list
    public class OrderView
    {
        public int OrderID { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Expected { get; set; }
    }
}
