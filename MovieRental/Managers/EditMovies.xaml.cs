using System;
using System.Collections.Generic;
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

namespace MovieRental.Managers
{
    /// <summary>
    /// Interaction logic for EditMovies.xaml
    /// </summary>
    public partial class EditMovies : Page
    {
        public EditMovies()
        {
            InitializeComponent();

            using (var context = new MovieRentalEntities())
            {
                var movies = from m in context.Movies
                             select m;

                MovieList.DisplayMemberPath = "Title";
                MovieList.ItemsSource = movies.ToList();

            }
        }

        private void MovieList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Movie current = (Movie) MovieList.SelectedItem;
            List<string> info = new List<string>();

            using (var context = new MovieRentalEntities())
            {
                var movie = context.Movies.Where(m => m.MovieID == current.MovieID).Single();

                info.Add("Genre " + movie.Genre.ToString());
                info.Add("Distribution Fee: $" + movie.DistributionFee.ToString("0.00"));
                info.Add("Number of copies: " + movie.NumberOfCopies.ToString());
                info.Add("Rating: " + movie.Rating.ToString());
                info.Add("Orders this week: " + GetWeeklyOrders(movie.MovieID));
                info.Add("Orders this month: " + GetMonthlyOrders(movie.MovieID));
                info.Add("Orders this year: " + GetYearlyOrders(movie.MovieID));
            }

            MovieInfo.ItemsSource = info;
        }

        private int GetWeeklyOrders(int movieID)
        {
            // Sunday of this week
            DayOfWeek firstDay = 0;

            DateTime firstOfWeek = DateTime.Today.Date;

            // Find the date of this week's Sunday
            while (firstOfWeek.DayOfWeek != firstDay)
            {
                firstOfWeek = firstOfWeek.AddDays(-1);
            }

            DateTime date = firstOfWeek.Date;

            using (var context = new MovieRentalEntities())
            {
                var query = "SELECT COUNT(OrderID) as NumberOfOrders FROM dbo.Orders WHERE Orders.RentalDate >= @date AND MovieID = @id";
                var weekRentals = context.Database.SqlQuery<int>(query, new SqlParameter("@date", date), 
                    new SqlParameter("@id", movieID)).Single();

                return weekRentals;
            }
        }

        private int GetMonthlyOrders(int movieID) 
        {
            DateTime firstOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            using (var context = new MovieRentalEntities())
            {
                var query = "SELECT COUNT(OrderID) as NumberOfOrders FROM dbo.Orders WHERE Orders.RentalDate >= @date AND MovieID = @id";
                var monthRentals = context.Database.SqlQuery<int>(query, new SqlParameter("@date", firstOfMonth),
                    new SqlParameter("@id", movieID)).Single();

                return monthRentals;
            }
        }

        private int GetYearlyOrders(int movieID)
        {
            DateTime firstOfYear = new DateTime(DateTime.Now.Year, 1, 1);

            using (var context = new MovieRentalEntities())
            {
                var query = "SELECT COUNT(OrderID) as NumberOfOrders FROM dbo.Orders WHERE Orders.RentalDate >= @date AND MovieID = @id";
                var yearRentals = context.Database.SqlQuery<int>(query, new SqlParameter("@date", firstOfYear),
                    new SqlParameter("@id", movieID)).Single();

                return yearRentals;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Movie current = (Movie)MovieList.SelectedItem;

            if (current != null)
            {
                using (var context = new MovieRentalEntities())
                {

                    if (MessageBox.Show("Are you sure you want to delete " + current.Title + "?", "Delete Movie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // Remove all references to the movie
                        var movie = context.Movies.Where(m => m.MovieID == current.MovieID).Single();
                        context.Movies.Remove(movie);

                        var orders = context.Orders.Where(o => o.MovieID == current.MovieID);

                        foreach (Order order in orders)
                        {
                            context.Orders.Remove(order);
                        }

                        var queues = context.Queues.Where(q => q.MovieID == current.MovieID);

                        foreach (Queue queue in queues)
                        {
                            context.Queues.Remove(queue);
                        }

                        var credits = context.Credits.Where(c => c.MovieID == current.MovieID);

                        foreach (Credit credit in credits)
                        {
                            context.Credits.Remove(credit);
                        }

                        context.SaveChanges();
                        MessageBox.Show(current.Title + " deleted");
                    }
                    else
                    {
                        return;
                    }
                    
                }
            }
        }
    }
}
