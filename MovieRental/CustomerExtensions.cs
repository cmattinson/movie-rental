using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental
{
    public partial class Customer
    {
        public enum Account
        {
            Limited = 0,
            Unlimited1 = 1,
            Unlimited2 = 2,
            Unlimited3 = 3
        }

        public virtual string FullName { get { return FirstName + " " + LastName; } }
    }
}
