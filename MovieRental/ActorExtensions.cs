using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental
{
    public partial class Actor
    {
        public virtual string FullName { get { return FirstName + " " + LastName; } }
    }
}
