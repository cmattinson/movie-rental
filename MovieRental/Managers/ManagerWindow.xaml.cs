using MovieRental.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MovieRental
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        Employee manager;

        public ManagerWindow(Employee manager)
        {
            InitializeComponent();
            this.manager = manager;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new AddMovie());
        }

        // Set frame to add movie screen
        private void AddMovies_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new AddMovie());
        }

        private void ManageEmployees_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new ManageEmployees());
        }

        private void SalesReports_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new SalesReports());
        }

        private void BrowseMovies_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new EditMovies());
        }

        private void Rentals_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new BrowseRentals());
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new Statistics());
        }
    }
}

           