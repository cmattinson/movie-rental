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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        Customer customer;
        public Settings(Customer customer)
        {
            InitializeComponent();
            this.customer = customer;

            FirstNameBox.Text = customer.FirstName;
            LastNameBox.Text = customer.LastName;
            AddressBox.Text = customer.Address;
            CityBox.Text = customer.City;
            ProvinceBox.Text = customer.Province;
            PostalCodeBox.Text = customer.PostalCode;
            PhoneNumberBox.Text = customer.Phone;
            EmailBox.Text = customer.Email;
        }

        // TODO: Username and password changing
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MovieRentalEntities())
            {
                var customer = context.Customers.SingleOrDefault(c => c.AccountNumber == this.customer.AccountNumber);

                if (customer != null)
                {
                    if (!string.IsNullOrWhiteSpace(FirstNameBox.Text)) 
                    {
                        customer.FirstName = FirstNameBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(LastNameBox.Text))
                    {
                        customer.LastName = LastNameBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(AddressBox.Text))
                    {
                        customer.Address = AddressBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(CityBox.Text))
                    {
                        customer.City = CityBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(ProvinceBox.Text))
                    {
                        customer.Province = ProvinceBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(PostalCodeBox.Text))
                    {
                        customer.PostalCode = PostalCodeBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(PhoneNumberBox.Text))
                    {
                        customer.Phone = PhoneNumberBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(EmailBox.Text))
                    {
                        customer.Email = EmailBox.Text;
                    }

                    context.SaveChanges();
                    MessageBox.Show("Changes saved");
                }
            }

        }
    }
}
