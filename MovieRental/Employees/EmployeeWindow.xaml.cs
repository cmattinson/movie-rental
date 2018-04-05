using MovieRental.Employees;
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
    /// Interaction logic for EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        Employee employee;

        public EmployeeWindow(Employee employee)
        {
            InitializeComponent();
            this.employee = employee;

            Frame.NavigationService.Navigate(new BrowseOrders(employee));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }

        private void Customers_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(new ManageCustomers());
        }
    }
}
