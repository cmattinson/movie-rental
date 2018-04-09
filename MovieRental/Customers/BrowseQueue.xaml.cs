using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
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
    /// Interaction logic for BrowseQueue.xaml
    /// </summary>
    public partial class BrowseQueue : Page
    {
        Customer customer;

        public BrowseQueue(Customer customer)
        {
            InitializeComponent();
            this.customer = customer;

            using (var context = new MovieRentalEntities())
            {
                var queues = context.Queues.Where(q => q.AccountNumber == customer.AccountNumber).ToList();


                foreach (Queue queue in queues)
                {
                    var movie = context.Movies.Where(q => q.MovieID == queue.MovieID).Single();

                    Queue.Items.Add(movie.Title + " - Added on " + queue.DateAdded.ToShortDateString());
                }
            }
        }

        //private void Rent_Click(object sender, RoutedEventArgs e)
        //{
            

        //    using (var context = new MovieRentalEntities())
        //    {
        //        // The first day of the current month
        //        DateTime firstOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        //        var countMonth = "SELECT COUNT(*) FROM dbo.Orders WHERE RentalDate > @date AND AccountNumber = @account";
        //        var countRequests = "SELECT COUNT(*) FROM dbo.Orders WHERE RentalDate IS NULL AND AccountNumber = @account";
        //        var countCurrent = "SELECT COUNT(*) FROM dbo.Orders WHERE AccountNumber = @account AND RentalDate > @date AND ActualReturn IS NULL";

        //        var monthlyOrders = context.Database.SqlQuery<int>(countMonth, new SqlParameter("@date", firstOfMonth),
        //            new SqlParameter("@account", customer.AccountNumber)).Single();

        //        var requests = context.Database.SqlQuery<int>(countRequests, new SqlParameter("@account", customer.AccountNumber)).Single();

        //        var currentOrders = context.Database.SqlQuery<int>(countCurrent, new SqlParameter("@account", customer.AccountNumber),
        //            new SqlParameter("@date", firstOfMonth)).Single();

        //        Console.WriteLine(currentOrders);

        //        int account = customer.AccountType;

        //        if ((monthlyOrders == 1 || requests == 1) && account == 0)
        //        {
        //            MessageBox.Show("You have already rented your movie for the month");
        //            return;
        //        }

        //        if (account == 1)
        //        {
        //            if (currentOrders == 1 || requests == 1)
        //            {
        //                MessageBox.Show("You can only rent one movie at a time. Please return a movie or wait for your previous orders to be approved.");
        //                return;
        //            }
        //        }
        //        if (account == 2)
        //        {
        //            if (currentOrders == 2 || requests == 2)
        //            {
        //                MessageBox.Show("You can only rent two movies at a time. Please return a movie or wait for your previous orders to be approved.");
        //                return;
        //            }
        //        }
        //        if (account == 3)
        //        {
        //            if (currentOrders == 3 || requests == 3)
        //            {
        //                MessageBox.Show("You can only rent three movies at a time. Please return a movie or wait for your previous orders to be approved.");
        //                return;
        //            }
        //        }


        //        // Order to be approved by an employee
        //        Order order = new Order()
        //        {
        //            MovieID = current.MovieID,
        //            AccountNumber = customer.AccountNumber,
        //        };

        //        try
        //        {
        //            context.Orders.Add(order);
        //            context.SaveChanges();

        //            MessageBox.Show("Your request has been sent");
        //        }
        //        catch (DbUpdateException)
        //        {

        //        }
        //    }
        //}
    }
}
