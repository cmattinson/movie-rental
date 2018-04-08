using System;
using System.Collections.Generic;
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

namespace MovieRental.Customers
{
    /// <summary>
    /// Interaction logic for BrowseOrders.xaml
    /// </summary>
    public partial class BrowseOrders : Page
    {
        Customer customer;

        public BrowseOrders(Customer customer)
        {
            InitializeComponent();
            this.customer = customer;

            var gridView = new GridView();
            this.OrderList.View = gridView;

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "OrderID",
                DisplayMemberBinding = new Binding("OrderID")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Movie",
                DisplayMemberBinding = new Binding("Title")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Rental Date",
                DisplayMemberBinding = new Binding("Date")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Expected Return",
                DisplayMemberBinding = new Binding("Expected")
            });

            using (var context = new MovieRentalEntities())
            {
                // Orders that have been approved and not returned yet
                var orders = context.Orders.Include("Movie").Where(o => o.AccountNumber == customer.AccountNumber && o.RentalDate != null 
                && o.ActualReturn == null).ToList();

                foreach (Order order in orders)
                {
                    OrderList.Items.Add(new OrderView { OrderID = order.OrderID, Title = order.Movie.Title,
                        Date = order.RentalDate.Value.ToShortDateString(), Expected = order.ExpectedReturn.Value.ToShortDateString() });
                }
            }

            using (var context = new MovieRentalEntities())
            {
                // Orders that have been returned
                var history = context.Orders.Include("Movie").Where(o => o.AccountNumber == customer.AccountNumber && o.ActualReturn != null).ToList();

                History.DisplayMemberPath = "CustomerOrderInfo";
                History.ItemsSource = history;
            }
        }

        private void OrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            OrderView current = (OrderView)OrderList.SelectedItem;

            using (var context = new MovieRentalEntities())
            {
                var order = context.Orders.Include("Movie").SingleOrDefault(o => o.OrderID == current.OrderID);

                order.ActualReturn = DateTime.Today;
                order.Movie.NumberOfCopies++;
                context.SaveChanges();
                MessageBox.Show("Movie has been returned");
            }
        }

        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            Order selected = (Order)History.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Please select a movie to rate");
            }
            else
            {
                using (var context = new MovieRentalEntities())
                {
                    var movie = context.Movies.Where(m => m.MovieID == selected.MovieID);


                }
            }
        }
    }
}
