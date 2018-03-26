using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental
{
    public partial class Order
    {
        public virtual string OrderInfo { get { return "Account " + AccountNumber + " - " + Movie.Title; } }
    }
}
