using MovieRental.Customers;
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
using System.Windows.Shapes;

namespace MovieRental
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        Customer customer;

        public CustomerWindow(Customer customer)
        {
            InitializeComponent();
            this.customer = customer;

            Frame.NavigationService.Navigate(new BrowseMovies(customer));
        }

        public CustomerWindow()
        {
            InitializeComponent();
        }

        private void BrowseMovies_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new BrowseMovies(customer));
        }

        private void BrowseQueue_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new BrowseQueue(customer));
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new BrowseOrders(customer));
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new Settings());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }
    }
}
