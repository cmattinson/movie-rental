using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using static MovieRental.Customer;

namespace MovieRental.Employees
{
    /// <summary>
    /// Interaction logic for BrowseOrders.xaml
    /// </summary>
    public partial class BrowseOrders : Page
    {
        Employee employee;

        public BrowseOrders(Employee employee)
        {
            InitializeComponent();
            this.employee = employee;

            OrderList.DisplayMemberPath = "OrderInfo";
            OrderList.ItemsSource = GetOrders();
        }

        public List<Order> GetOrders()
        {
            using (var context = new MovieRentalEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var orders = context.Orders.Include("Movie").Include("Customer").Where(order => order.RentalDate == null).ToList();
                return orders;
            }
        }

        private void OrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Order current = (Order)OrderList.SelectedItem;

            List<string> info = new List<string>();

            info.Add("Customer - " + current.Customer.FirstName + " " + current.Customer.LastName);
            info.Add("Movie - " + current.Movie.Title);
            info.Add("Number of Copies - " + current.Movie.NumberOfCopies.ToString());

            Account account = (Account)current.Customer.AccountType;
            info.Add("Account type - " + account);

            OrderInfo.ItemsSource = info;
        }

        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            Order current = (Order)OrderList.SelectedItem;

            using (var context = new MovieRentalEntities())
            {
                var order = context.Orders.SingleOrDefault(o => o.OrderID == current.OrderID);

                if (order != null)
                {
                    order.SIN = employee.SIN;
                    order.RentalDate = System.DateTime.Today;
                    order.ExpectedReturn = System.DateTime.Today.AddMonths(1);
                    context.SaveChanges();
                }
            }
        }
    }
}
