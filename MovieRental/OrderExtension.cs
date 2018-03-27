using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental
{
    public partial class Order
    {
        public virtual string EmployeeOrderInfo { get { return "Account " + AccountNumber + " - " + Movie.Title; } }
        public virtual string CustomerOrderInfo { get { return Movie.Title + " - " + RentalDate.ToString(); } }
    }
}
