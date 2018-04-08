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
    /// Interaction logic for ManageEmployees.xaml
    /// </summary>
    public partial class ManageEmployees : Page
    {
        public ManageEmployees()
        {
            InitializeComponent();

            using (var context = new MovieRentalEntities())
            {
                var employees = from e in context.Employees select e;
                EmployeeList.DisplayMemberPath = "FirstName";

                EmployeeList.ItemsSource = employees.ToList();
                EmployeeList.SelectedIndex = -1;
            }

            AccountType.Items.Add("Employee"); AccountType.Items.Add("Manager");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MovieRentalEntities())
            {
                if (PasswordBox.Password == ConfirmBox.Password)
                {
                    string position = AccountType.SelectedItem.ToString();
                    int type;

                    if (position == "Employee")
                    {
                        type = 1;
                    }
                    else
                    {
                        type = 0;
                    }

                    Employee employee = new Employee()
                    {
                        SIN = SINBox.Text,
                        FirstName = FirstNameBox.Text,
                        LastName = LastNameBox.Text,
                        Address = AddressBox.Text,
                        City = CityBox.Text,
                        Province = ProvinceBox.Text,
                        PostalCode = PostalCodeBox.Text,
                        Phone = PhoneBox.Text,
                        StartDate = DateTime.Today,
                        Wage = Convert.ToDecimal(WageBox.Text),
                        Username = UsernameBox.Text,
                        Password = PasswordBox.Password,
                        AccountType = type
                    };

                    context.Employees.Add(employee);
                    context.SaveChanges();

                    MessageBox.Show("Employee added");
                }

            }

        }

        private void EmployeeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeList.SelectedIndex != -1)
            {
                Employee selected = (Employee)EmployeeList.SelectedItem;
                Add.Visibility = Visibility.Hidden;

                SINBox.Text = selected.SIN;
                FirstNameBox.Text = selected.FirstName;
                LastNameBox.Text = selected.LastName;
                AddressBox.Text = selected.Address;
                CityBox.Text = selected.City;
                ProvinceBox.Text = selected.Province;
                PostalCodeBox.Text = selected.PostalCode;
                PhoneBox.Text = selected.Phone;
                WageBox.Text = selected.Wage.ToString("0.00");
                UsernameBox.Text = selected.Username;

                if (selected.AccountType == 0)
                {
                    AccountType.SelectedItem = "Manager";
                }
                else
                {
                    AccountType.SelectedItem = "Employee";
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Employee selected = (Employee)EmployeeList.SelectedItem;

            using (var context = new MovieRentalEntities())
            {
                var employee = context.Employees.Where(emp => emp.SIN == selected.SIN).Single();

                if (employee != null)
                {
                    if (!string.IsNullOrWhiteSpace(SINBox.Text))
                    {
                        employee.SIN = SINBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(FirstNameBox.Text))
                    {
                        employee.FirstName = FirstNameBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(LastNameBox.Text))
                    {
                        employee.LastName = LastNameBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(AddressBox.Text))
                    {
                        employee.Address = AddressBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(CityBox.Text))
                    {
                        employee.City = CityBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(ProvinceBox.Text))
                    {
                        employee.Province = ProvinceBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(PostalCodeBox.Text))
                    {
                        employee.PostalCode = PostalCodeBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(PhoneBox.Text))
                    {
                        employee.Phone = PhoneBox.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(WageBox.Text))
                    {
                        employee.Wage = Convert.ToDecimal(WageBox.Text);
                    }
                    if (!string.IsNullOrWhiteSpace(UsernameBox.Text))
                    {
                        employee.Username = UsernameBox.Text;
                    }

                    if (!string.IsNullOrWhiteSpace(PasswordBox.Password) && PasswordBox.Password == ConfirmBox.Password)
                    {
                        employee.Password = PasswordBox.Password;
                    }

                    if(AccountType.SelectedIndex != -1)
                    {
                        string position = AccountType.SelectedItem.ToString();

                        if (position == "Employee")
                        {
                            employee.AccountType = 1;
                        }
                        else
                        {
                            employee.AccountType = 0;
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show("Changes saved");
                }
            }

            Add.Visibility = Visibility.Visible;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SINBox.Text = null;
            FirstNameBox.Text = null;
            LastNameBox.Text = null;
            AddressBox.Text = null;
            CityBox.Text = null;
            ProvinceBox.Text = null;
            PostalCodeBox.Text = null;
            PhoneBox.Text = null;
            WageBox.Text = null;
            UsernameBox.Text = null;
            AccountType.SelectedIndex = -1;
            EmployeeList.SelectedIndex = -1;

            Add.Visibility = Visibility.Visible;
        }
    }
}
