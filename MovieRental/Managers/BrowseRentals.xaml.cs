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

namespace MovieRental.Managers
{
    /// <summary>
    /// Interaction logic for BrowseRentals.xaml
    /// </summary>
    public partial class BrowseRentals : Page
    {
        public BrowseRentals()
        {
            InitializeComponent();

            using (var context = new MovieRentalEntities())
            {
                var orders = context.Orders.Include("Movie").Include("Customer").ToList();

                OrderList.DisplayMemberPath = "EmployeeOrderInfo";
                OrderList.ItemsSource =  orders;
            }

            SearchBy.Items.Add("Customer");
            SearchBy.Items.Add("Movie Title");
            SearchBy.Items.Add("Genre");

            Genres.ItemsSource = GenreDict.genreDict;
            Genres.SelectedValuePath = "Value";
            Genres.DisplayMemberPath = "Value";
            Genres.SelectedIndex = 0;

            SearchBox.Visibility = Visibility.Hidden;
            Genres.Visibility = Visibility.Hidden;
            SearchButton.Visibility = Visibility.Hidden;
            FirstName.Visibility = Visibility.Hidden;
            LastName.Visibility = Visibility.Hidden;
        }

        private void SearchBy_DropDownClosed(object sender, EventArgs e)
        {
            string current = (string)SearchBy.SelectedItem;

            switch (current)
            {
                case "Customer":
                    FirstName.Visibility = Visibility.Visible;
                    LastName.Visibility = Visibility.Visible;
                    SearchBox.Visibility = Visibility.Hidden;
                    SearchButton.Visibility = Visibility.Visible;
                    Genres.Visibility = Visibility.Hidden;
                    break;
                case "Genre":
                    Genres.Visibility = Visibility.Visible;
                    SearchBox.Visibility = Visibility.Hidden;
                    SearchButton.Visibility = Visibility.Hidden;
                    FirstName.Visibility = Visibility.Hidden;
                    LastName.Visibility = Visibility.Hidden;
                    break;
                case "Movie Title":
                    SearchBox.Visibility = Visibility.Visible;
                    SearchButton.Visibility = Visibility.Visible;
                    Genres.Visibility = Visibility.Hidden;
                    FirstName.Visibility = Visibility.Hidden;
                    LastName.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void Genres_DropDownClosed(object sender, EventArgs e)
        {
            string genre = Genres.SelectedValue.ToString();

            using (var context = new MovieRentalEntities())
            {
                var orders = context.Orders.Include("Movie").Where(o => o.Movie.Genre == genre).ToList();

                OrderList.ItemsSource = orders;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string search = SearchBox.Text;

            using (var context = new MovieRentalEntities())
            {
                string current = (string)SearchBy.SelectedItem;

                switch (current)
                {
                    case "Customer":
                        if (!string.IsNullOrWhiteSpace(FirstName.Text) && !string.IsNullOrWhiteSpace(LastName.Text))
                        {

                            var customerOrders = context.Orders.Include("Customer").Include("Movie").Where(o => o.Customer.FirstName.Contains(FirstName.Text) &&
                            o.Customer.LastName.Contains(LastName.Text)).ToList();
                            OrderList.ItemsSource = customerOrders;
                            break;
                        }
                        else if (!string.IsNullOrWhiteSpace(FirstName.Text))
                        {
                            var customerOrders = context.Orders.Include("Customer").Include("Movie").Where(o => o.Customer.FirstName.Contains(FirstName.Text)).ToList();
                            OrderList.ItemsSource = customerOrders;
                            break;
                        }
                        else if (!string.IsNullOrWhiteSpace(LastName.Text))
                        {
                            var customerOrders = context.Orders.Include("Customer").Include("Movie").Where(o => o.Customer.LastName.Contains(LastName.Text)).ToList();
                            OrderList.ItemsSource = customerOrders;
                            break;
                        }
                        else
                        {
                            OrderList.ItemsSource = null;
                            break;
                        }
                    case "Movie Title":
                        var movieRentals = context.Orders.Include("Movie").Where(o => o.Movie.Title.Contains(search)).ToList();
                        OrderList.ItemsSource = movieRentals;
                        break;
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MovieRentalEntities())
            {
                var orders = context.Orders.Include("Movie").Include("Customer").ToList();

                OrderList.ItemsSource = orders;

            }
        }

        private void OrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RentalInfo.Items.Clear();

            Order order = (Order)OrderList.SelectedItem;

            using (var context = new MovieRentalEntities())
            {
                var movie = context.Movies.Where(m => m.MovieID == order.MovieID).Single();

                RentalInfo.Items.Add("Movie: " + movie.Title);
                RentalInfo.Items.Add("Customer: " + order.Customer.FullName);
                RentalInfo.Items.Add("Rental Date: " + order.RentalDate.ToString());
                RentalInfo.Items.Add("Return Date: " + order.ActualReturn.ToString());
            }
        }
    }
}
