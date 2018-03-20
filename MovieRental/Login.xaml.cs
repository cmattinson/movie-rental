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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Text;
            Customer customer;

            if (IsCustomer(username))
            {
                using (var context = new MovieRentalEntities())
                {
                    customer = context.Customers.Where(u => u.Username == username).FirstOrDefault();
                }

                var customerScreen = new CustomerWindow(customer);
                customerScreen.Show();
                this.Close();
            }
        }

        private bool IsCustomer(string username)
        {
            using (var context = new MovieRentalEntities())
            {
                try
                {
                    var query = context.Customers.Where(u => u.Username == UsernameBox.Text).FirstOrDefault();
                    
                    if (query.Password == PasswordBox.Text)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect password");
                        return false;
                    }
                }
                catch (System.NullReferenceException)
                {
                    return false;
                }
            }
        }
    }
}
