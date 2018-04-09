using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieRental.Managers
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Page
    {
        Dictionary<string, int> months = new Dictionary<string, int>();

        public Statistics()
        {
            InitializeComponent();

            

            months.Add("January", 1);
            months.Add("February", 2);
            months.Add("March", 3);
            months.Add("April", 4);
            months.Add("May", 5);
            months.Add("June", 6);
            months.Add("July", 7);
            months.Add("August", 8);
            months.Add("September", 9);
            months.Add("October", 10);
            months.Add("November", 11);
            months.Add("December", 12);

            Month.ItemsSource = months;
            Month.DisplayMemberPath = "Key";
            Month.SelectedValuePath = "Value";

            Month.SelectedIndex = 0;

            MonthHeader.Text = "Statistics for January " + DateTime.Today.Year.ToString();
            CustomersHeader.Text = "Top Customers";
        }

        private void Month_DropDownClosed(object sender, EventArgs e)
        {
            Stats.Items.Clear();

            
            int month = (int)Month.SelectedValue;
            string monthString = months.FirstOrDefault(m => m.Value == month).Key;

            MonthHeader.Text = "Statistics for " + monthString + " " + DateTime.Today.Year.ToString();

            DateTime firstOfMonth = new DateTime(DateTime.Today.Year, month, 1);

            DateTime lastOfMonth;

            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) // 31 day months
            {
                lastOfMonth = new DateTime(DateTime.Today.Year, month, 31);
            }
            else if (month == 4 || month == 6 || month == 9 || month == 11) // 30 day months
            {
                lastOfMonth = new DateTime(DateTime.Today.Year, month, 30);
            }
            else if (month == 2) // February
            {
                if (DateTime.IsLeapYear(DateTime.Today.Year))
                {
                    lastOfMonth = new DateTime(DateTime.Today.Year, month, 29);
                }
                else
                {
                    lastOfMonth = new DateTime(DateTime.Today.Year, month, 28);
                }
            }
            else
            {
                return;
            }

            using (var context = new MovieRentalEntities())
            {
                var accQuery = "SELECT AccountType, COUNT(Customer.AccountNumber) as Count FROM Customer GROUP BY AccountType";

                Employee topEmployee = GetTopEmployee(firstOfMonth, lastOfMonth);
                List<Customer> topCustomers = GetTopCustomers(firstOfMonth, lastOfMonth);
                var accounts = context.Database.SqlQuery<AccountTypeCount>(accQuery);

                TopCustomerList.ItemsSource = topCustomers;
                TopCustomerList.DisplayMemberPath = "FullName";

                int income = 0;

                foreach(AccountTypeCount result in accounts)
                {
                    if (result.AccountType == 0)
                    {
                        income += (10 * result.Count);
                    }
                    else if (result.AccountType == 1)
                    {
                        income += (15 * result.Count);
                    }
                    else if (result.AccountType == 2)
                    {
                        income += (20 * result.Count);
                    }
                    else
                    {
                        income += (25 * result.Count);
                    }
                }

                if (topEmployee != null)
                {
                    Stats.Items.Add("Top employee: " + topEmployee.FirstName + " " + topEmployee.LastName);
                    Stats.Items.Add("Total income: $" + Convert.ToDecimal(income));
                }
                else
                {
                    Stats.Items.Add("No rentals for this month");
                    return;
                }
            }
        }

        public Employee GetTopEmployee(DateTime firstOfMonth, DateTime lastOfMonth)
        {
            TopEmployee employee;

            using (var context = new MovieRentalEntities())
            {
                var empQuery = "SELECT Employee.SIN, COUNT(OrderID) as NumberOfOrders FROM dbo.Employee,dbo.Orders WHERE Employee.SIN = Orders.SIN AND Orders.RentalDate >= @first AND Orders.RentalDate <= @last GROUP BY Employee.SIN ORDER BY (NumberOfOrders) DESC";
                SqlParameter first = new SqlParameter("@first", firstOfMonth);
                SqlParameter last = new SqlParameter("@last", lastOfMonth);

                var topEmployee = context.Database.SqlQuery<TopEmployee>(empQuery, first, last).ToList();

                if (topEmployee.Count() != 0)
                {
                    employee = topEmployee.First();

                    Employee top = context.Employees.Where(emp => emp.SIN == employee.SIN).Single();

                    return top;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<Customer> GetTopCustomers(DateTime firstOfMonth, DateTime lastOfMonth)
        {
            List<Customer> topCustomers = new List<Customer>();

            using (var context = new MovieRentalEntities())
            {
                var cusQuery = "SELECT Customer.AccountNumber, COUNT(OrderID) as NumberOfOrders FROM Customer,Orders WHERE Customer.AccountNumber = Orders.AccountNumber AND Orders.RentalDate >= @first AND Orders.RentalDate <= @last GROUP BY Customer.AccountNumber ORDER BY (NumberOfOrders) DESC";
                SqlParameter first = new SqlParameter("@first", firstOfMonth);
                SqlParameter last = new SqlParameter("@last", lastOfMonth);

                var results = context.Database.SqlQuery<TopCustomer>(cusQuery, first, last).ToList();

                if (results.Count() != 0)
                {
                    foreach (TopCustomer cust in results)
                    {
                        Customer top = context.Customers.Where(c => c.AccountNumber == cust.AccountNumber).Single();
                        topCustomers.Add(top);
                    }

                    return topCustomers;
                }
                else
                {
                    Console.WriteLine("Is null");
                    return null;
                }
            }
        }
    }
}
