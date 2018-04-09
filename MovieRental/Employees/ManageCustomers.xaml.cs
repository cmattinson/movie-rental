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

namespace MovieRental.Employees
{
    /// <summary>
    /// Interaction logic for ManageCustomers.xaml
    /// </summary>
    public partial class ManageCustomers : Page
    {

        public ManageCustomers(Customer customer)
        {
            InitializeComponent();

            using (var context = new MovieRentalEntities())
            {
                var customers = from c in context.Customers
                               select c;

                CustomerList.DisplayMemberPath = "FirstName";
                CustomerList.SelectedValuePath = "AccountNumber";

                CustomerList.ItemsSource = customers.ToList();
                CustomerList.SelectedIndex = 0;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MovieRentalEntities())
            {
                Customer customer = new Customer();
                {
                    customer.FirstName = FirstNameBox.Text;
                    customer.LastName = LastNameBox.Text;
                    customer.Address = AddressBox.Text;
                    customer.City = CityBox.Text;
                    customer.Province = ProvinceBox.Text;
                    customer.PostalCode = PostalCodeBox.Text;
                    customer.Phone = PhoneNumberBox.Text;
                    customer.Email = EmailBox.Text;
                    customer.CreditCard = CreditCardBox.Text;
                    customer.Username = UsernameBox.Text;
                    customer.Password = PasswordBox.Password;
                    customer.Rating = 0;
                    customer.CreationDate = DateTime.Today;

                    int value = int.Parse(AccountTypeBox.Text);
                    customer.AccountType = value;
                    PasswordBox.PasswordChar = '•';
                }

                context.Customers.Add(customer);
                context.SaveChanges();
                MessageBox.Show("Customer added");
            }
        }


        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Customer selected = (Customer)CustomerList.SelectedItem;

            if (selected != null)
            {
                using (var context = new MovieRentalEntities())
                {

                    if (MessageBox.Show("Are you sure you want to delete " + selected.FirstName + " " + selected.LastName + "?", "Delete Customer", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var customer = context.Customers.Where(cus => cus.AccountNumber == selected.AccountNumber).Single();
                        context.Customers.Remove(customer);

                        context.SaveChanges();
                        MessageBox.Show(selected.FirstName + " " + selected.LastName + " deleted");
                    }
                    else
                    {
                        return;
                    }

                }
            }
        }

        private void CustomerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerList.SelectedIndex != -1)
            {
                Customer selected = (Customer)CustomerList.SelectedItem;
               
                FirstNameBoxEdit.Text = selected.FirstName;
                LastNameBoxEdit.Text = selected.LastName;
                AddressBoxEdit.Text = selected.Address;
                CityBoxEdit.Text = selected.City;
                ProvinceBoxEdit.Text = selected.Province;
                PostalCodeBoxEdit.Text = selected.PostalCode;
                PhoneNumberBoxEdit.Text = selected.Phone;
                EmailBoxEdit.Text = selected.Email;
                CreditCardBoxEdit.Text = selected.CreditCard;
                UsernameBoxEdit.Text = selected.Username;
                PasswordBoxEdit.Password = selected.Password;

                string type = selected.AccountType.ToString();
                AccountTypeBoxEdit.Text = type;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Customer selected = (Customer)CustomerList.SelectedItem;

            using (var context = new MovieRentalEntities())
            {
                var customer = context.Customers.Where(cus => cus.AccountNumber == selected.AccountNumber).Single();

                if (customer != null)
                {
                    if (!string.IsNullOrWhiteSpace(EmailBoxEdit.Text))
                    {
                        customer.Email = EmailBoxEdit.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(FirstNameBoxEdit.Text))
                    {
                        customer.FirstName = FirstNameBoxEdit.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(LastNameBoxEdit.Text))
                    {
                        customer.LastName = LastNameBoxEdit.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(AddressBoxEdit.Text))
                    {
                        customer.Address = AddressBoxEdit.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(CityBoxEdit.Text))
                    {
                        customer.City = CityBoxEdit.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(ProvinceBoxEdit.Text))
                    {
                        customer.Province = ProvinceBoxEdit.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(PostalCodeBoxEdit.Text))
                    {
                        customer.PostalCode = PostalCodeBoxEdit.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(PhoneNumberBoxEdit.Text))
                    {
                        customer.Phone = PhoneNumberBoxEdit.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(AccountTypeBoxEdit.Text))
                    {
                        int value = int.Parse(AccountTypeBoxEdit.Text);
                        customer.AccountType = value;
                    }
                    if (!string.IsNullOrWhiteSpace(UsernameBox.Text))
                    {
                        customer.Username = UsernameBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                    {
                        customer.Password = PasswordBox.Password;
                    }

                    context.SaveChanges();
                    MessageBox.Show("Changes saved");
                }
            }
        }
    }
}