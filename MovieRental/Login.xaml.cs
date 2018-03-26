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
            PasswordBox.PasswordChar = '•';
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            Customer customer;
            Employee employee;
            Employee manager;

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
            else if (IsEmployee(username))
            {
                using (var context = new MovieRentalEntities())
                {
                    employee = context.Employees.Where(u => u.Username == username).FirstOrDefault();
                }

                var employeeScreen = new EmployeeWindow(employee);
                employeeScreen.Show();
                this.Close();
            }
            else if (IsManager(username))
            {
                using (var context = new MovieRentalEntities())
                {
                    manager = context.Employees.Where(u => u.Username == username).FirstOrDefault();
                }

                var managerScreen = new ManagerWindow(manager);
                managerScreen.Show();
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
                    
                    if (query.Password == PasswordBox.Password)
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
                    Console.WriteLine("Not a customer");
                    return false;
                }
            }
        }

        private bool IsEmployee(string username)
        {
            using (var context = new MovieRentalEntities())
            {
                try
                {
                    var employee = context.Employees.Where(e => e.Username == UsernameBox.Text).FirstOrDefault();

                    if (employee.Password == PasswordBox.Password && employee.AccountType == 1) 
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (System.NullReferenceException)
                {
                    return false;
                }
            }
        }

        private bool IsManager(string username)
        {
            using (var context = new MovieRentalEntities())
            {
                try
                {
                    var employee = context.Employees.Where(e => e.Username == UsernameBox.Text).FirstOrDefault();

                    if (employee.Password == PasswordBox.Password && employee.AccountType == 0)
                    {
                        return true;
                    }
                    else
                    {
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
