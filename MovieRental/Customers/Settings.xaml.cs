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

namespace MovieRental.Customers
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        Customer customer;
        string currentPage;

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
            UsernameBox.Text = customer.Username;

            PasswordBox.PasswordChar = '•';
            ConfirmBox.PasswordChar = '•';


            AccountInfo.Text = "Your account is " + (Account)customer.AccountType;

            if (customer.AccountType == 0)
            {
                UpgradeFrame.NavigationService.Navigate(new Unlimited1());

            }
            else if (customer.AccountType == 1)
            {
                UpgradeFrame.NavigationService.Navigate(new Unlimited2());
            }
            else if (customer.AccountType == 2)
            {
                UpgradeFrame.NavigationService.Navigate(new Unlimited3());
            }
            else if (customer.AccountType == 3)
            {
                UpgradeFrame.NavigationService.Navigate(new Unlimited2());
            }
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
                    if (!string.IsNullOrWhiteSpace(UsernameBox.Text))
                    {
                        customer.Username = UsernameBox.Text;
                    }

                    if (!string.IsNullOrWhiteSpace(PasswordBox.Password) && PasswordBox.Password == ConfirmBox.Password)
                    {
                        customer.Password = PasswordBox.Password;
                    }

                    context.SaveChanges();
                    MessageBox.Show("Changes saved");
                }
            }

        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            currentPage = UpgradeFrame.Content.GetType().Name.ToString();

            // Current page is Unlimited 1, go to Limited and hide Previous button
            if (currentPage == "Unlimited1")
            {
                UpgradeFrame.NavigationService.Navigate(new Limited());
                Previous.Visibility = Visibility.Hidden;
            }
            // Current page is Unlimited 2, go to Unlimited 1
            if (currentPage == "Unlimited2")
            {
                UpgradeFrame.NavigationService.Navigate(new Unlimited1());
            }
            // Current page is Unlimited 3, go to Unlimited 2
            if (currentPage == "Unlimited3")
            {
                UpgradeFrame.NavigationService.Navigate(new Unlimited2());
                Next.Visibility = Visibility.Visible;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            currentPage = UpgradeFrame.Content.GetType().Name.ToString();

            // Current page is Limited, go to Unlimited 1
            if (currentPage == "Limited")
            {
                UpgradeFrame.NavigationService.Navigate(new Unlimited1());
                Previous.Visibility = Visibility.Visible;
            }
            // Current page is Unlimited 1, go to Unlimited 2
            if (currentPage == "Unlimited1")
            {
                UpgradeFrame.NavigationService.Navigate(new Unlimited2());
            }
            // Current page is Unlimited 2, go to Unlimited 3 and hide Next button
            if (currentPage == "Unlimited2")
            {
                UpgradeFrame.NavigationService.Navigate(new Unlimited3());
                Next.Visibility = Visibility.Hidden;
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            int newAccount;
            currentPage = UpgradeFrame.Content.GetType().Name.ToString();

            if (currentPage == "Limited")
            {
                if (MessageBox.Show("Are you sure you want to change to the Limited account?", "Account request", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    newAccount = 0;
                }
                else
                {
                    return;
                }
                
            }
            // Current page is Unlimited 1, go to Unlimited 2
            else if (currentPage == "Unlimited1")
            {
                if (MessageBox.Show("Are you sure you want to change to the Unlimited1 account?", "Account request", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    newAccount = 1;
                }
                else
                {
                    return;
                }
            }
            // Current page is Unlimited 2, go to Unlimited 3 and hide Next button
            else if (currentPage == "Unlimited2")
            {
                if (MessageBox.Show("Are you sure you want to change to the Unlimited2 account?", "Account request", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    newAccount = 2;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to change to the Unlimited3 account?", "Account request", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    newAccount = 3;
                }
                else
                {
                    return;
                }
            }

            using (var context = new MovieRentalEntities())
            {
                var customer = context.Customers.SingleOrDefault(c => c.AccountNumber == this.customer.AccountNumber);

                customer.AccountType = newAccount;
                context.SaveChanges();
            }
        }
    }
}
