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
using static MovieRental.Customer;

namespace MovieRental.Employees
{
    /// <summary>
    /// Interaction logic for ManageCustomers.xaml
    /// </summary>
    public partial class ManageCustomers : Page
    {

        public ManageCustomers()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MovieRentalEntities())
            {
                Customer customer = new Customer();
                {
                    FirstNameBox.Text = customer.FirstName;
                    LastNameBox.Text = customer.LastName;
                    AddressBox.Text = customer.Address;
                    CityBox.Text = customer.City;
                    ProvinceBox.Text = customer.Province;
                    PostalCodeBox.Text = customer.PostalCode;
                    PhoneNumberBox.Text = customer.Phone;
                    EmailBox.Text = customer.Email;
                    CreditCardBox.Text = customer.CreditCard;
                    UsernameBox.Text = customer.Username;
                    PasswordBox.Password = customer.Password;

                    int value = int.Parse(AccountTypeBox.Text);
                    value = customer.AccountType;
                    PasswordBox.PasswordChar = '•';
                }

                context.Customers.Add(customer);
                context.SaveChanges();
                MessageBox.Show("Customer added");
            }
        }
    }
}
