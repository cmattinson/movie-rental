using MovieRental.Customers;
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
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Search;
using APIMovie = TMDbLib.Objects.Movies.Movie;

namespace MovieRental
{
    /// <summary>
    /// Interaction logic for BrowseMovies.xaml
    /// </summary>
    public partial class BrowseMovies : Page
    {
        // Connect to api using api key
        TMDbClient client = new TMDbClient("ce183dd9fdee061774d69813580b16ea");
        Customer customer;

        public BrowseMovies(Customer customer)
        {
            InitializeComponent();
            this.customer = customer;

            SearchBy.Items.Add("Actor");
            SearchBy.Items.Add("Genre");
            SearchBy.Items.Add("Title");
            SearchBy.Items.Add("Most Popular");

            using (var context = new MovieRentalEntities())
            {
                var movies = from m in context.Movies
                             where m.NumberOfCopies > 0
                             select m;

                MovieList.DisplayMemberPath = "Title";
                MovieList.SelectedValuePath = "MovieID";

                MovieList.ItemsSource = movies.ToList();
                MovieList.SelectedIndex = 0;
            }

            Genres.ItemsSource = GenreDict.genreDict;
            Genres.SelectedValuePath = "Value";
            Genres.DisplayMemberPath = "Value";
            Genres.SelectedIndex = 0;

            List<string> spans = new List<string>();
            spans.Add("This week"); spans.Add("This month"); spans.Add("This year");
            Timespan.ItemsSource = spans;

            // Initially search by titles and hide the genre combobox
            SearchBy.SelectedIndex = 2;
            Genres.Visibility = Visibility.Hidden;
        }

        private void Movies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Movie current = (Movie) MovieList.SelectedItem;
                MovieTitle.Text = current.Title;

                APIMovie movie = client.GetMovieAsync(current.MovieID).Result;

                // Use the API to get the movie poster
                Uri apiUri = new Uri("http://image.tmdb.org/t/p/w342//");
                string posterPath = movie.PosterPath;
                System.UriBuilder uriBuilder = new System.UriBuilder(apiUri);
                uriBuilder.Path += posterPath;


                // TODO: Check if this is null
                Poster.Source = new BitmapImage(uriBuilder.Uri);

                MovieTitle.Text = current.Title;
                MovieOverview.Text = movie.Overview;
                GenreText.Text = current.Genre;

                int rating = current.Rating;

                if (rating >= 4)
                {
                    RatingCircle.Stroke = Brushes.Green;
                    RatingNumber.Text = rating.ToString();
                    RatingNumber.Foreground = Brushes.Green;

                }
                else if (rating >= 2 && rating < 4)
                {
                    RatingCircle.Stroke = Brushes.Gold;
                    RatingNumber.Text = rating.ToString();
                    RatingNumber.Foreground = Brushes.Gold;
                }
                else
                {
                    RatingCircle.Stroke = Brushes.Red;
                    RatingNumber.Text = rating.ToString();
                    RatingNumber.Foreground = Brushes.Red;
                }

                using (var context = new MovieRentalEntities())
                {
                    var query = context.Credits.Where(c => c.MovieID == current.MovieID).ToList();

                    List<string> actors = new List<string>();

                    foreach (Credit credit in query)
                    {
                        var actor = client.GetPersonAsync(credit.ActorID).Result;

                        actors.Add(actor.Name);
                    }

                    ActorList.ItemsSource = actors;
                }
            }
            catch (NullReferenceException error)
            {
                Console.WriteLine(error.Message);
            }
        }

        private void Queue_Click(object sender, RoutedEventArgs e)
        {
            Movie current = (Movie) MovieList.SelectedItem;

            using (var context = new MovieRentalEntities())
            {
                Queue queue = new Queue()
                {
                    AccountNumber = customer.AccountNumber,
                    MovieID = current.MovieID,
                    DateAdded = System.DateTime.Today
                };

                try
                {
                    context.Queues.Add(queue);
                    context.SaveChanges();
                    MessageBox.Show(current.Title + " has been added to your queue");
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show(current.Title + " is already in your queue");
                }
            }
        }

        private void Rent_Click(object sender, RoutedEventArgs e)
        {
            Movie current = (Movie) MovieList.SelectedItem;

            using (var context = new MovieRentalEntities())
            {
                // The first day of the current month
                DateTime firstOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                var countMonth = "SELECT COUNT(*) FROM dbo.Orders WHERE RentalDate > @date AND AccountNumber = @account";
                var countRequests = "SELECT COUNT(*) FROM dbo.Orders WHERE RentalDate IS NULL AND AccountNumber = @account";
                var countCurrent = "SELECT COUNT(*) FROM dbo.Orders WHERE AccountNumber = @account AND RentalDate > @date AND ActualReturn IS NULL";

                var monthlyOrders = context.Database.SqlQuery<int>(countMonth, new SqlParameter("@date", firstOfMonth), 
                    new SqlParameter("@account", customer.AccountNumber)).Single();

                var requests = context.Database.SqlQuery<int>(countRequests, new SqlParameter("@account", customer.AccountNumber)).Single();

                var currentOrders = context.Database.SqlQuery<int>(countCurrent, new SqlParameter("@account", customer.AccountNumber),
                    new SqlParameter("@date", firstOfMonth)).Single();

                Console.WriteLine(currentOrders);

                int account = customer.AccountType;

                if ((monthlyOrders == 1 || requests == 1) && account == 0)
                {
                    MessageBox.Show("You have already rented your movie for the month");
                    return;
                }

                if (account == 1)
                {
                    if (currentOrders == 1 || requests == 1)
                    {
                        MessageBox.Show("You can only rent one movie at a time. Please return a movie or wait for your previous orders to be approved.");
                        return;
                    }
                }
                if (account == 2)
                {
                    if (currentOrders == 2 || requests == 2)
                    {
                        MessageBox.Show("You can only rent two movies at a time. Please return a movie or wait for your previous orders to be approved.");
                        return;
                    }
                }
                if (account == 3)
                {
                    if (currentOrders == 3 || requests == 3)
                    {
                        MessageBox.Show("You can only rent three movies at a time. Please return a movie or wait for your previous orders to be approved.");
                        return;
                    }
                }


                // Order to be approved by an employee
                Order order = new Order()
                {
                    MovieID = current.MovieID,
                    AccountNumber = customer.AccountNumber,
                };

                try
                {
                    context.Orders.Add(order);
                    context.SaveChanges();

                    MessageBox.Show("Your request to rent " + current.Title + " has been sent");
                }
                catch (DbUpdateException)
                {

                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<Movie> movies = new List<Movie>();

            switch (SearchBy.SelectedItem)
            {
                case "Title":
                    movies = SearchByTitle(SearchBox.Text);

                    if (movies.Count() == 0)
                    {
                        MessageBox.Show(SearchBox.Text + " not found");
                    }
                    else
                    {
                        MovieList.DisplayMemberPath = "Title";
                        MovieList.ItemsSource = movies;
                        MovieList.SelectedIndex = 0;
                    }

                    break;
                case "Actor":
                    movies = SearchByActor(SearchBox.Text);

                    if (movies.Count() == 0)
                    {
                        MessageBox.Show("No movies with " + SearchBox.Text + " found");
                    }
                    else
                    {
                        MovieList.DisplayMemberPath = "Title";
                        MovieList.ItemsSource = movies;
                        MovieList.SelectedIndex = 0;
                    }

                    break;
                case "Genre":
                    movies = SearchByGenre(Genres.SelectedValue.ToString());

                    if (movies.Count() == 0)
                    {
                        MessageBox.Show("No movies in the " + Genres.SelectedValue.ToString() + " genre");
                    }
                    else
                    {
                        MovieList.DisplayMemberPath = "Title";
                        MovieList.ItemsSource = movies;
                        MovieList.SelectedIndex = 0;
                    }
                    break;

                case "Most Popular":
                    switch (Timespan.SelectedItem.ToString())
                    {
                        case "This week":
                            movies = GetWeekPopular();

                            if (movies.Count() == 0)
                            {
                                MessageBox.Show("No orders have been made this week");
                            }
                            else
                            {
                                MovieList.DisplayMemberPath = "Title";
                                MovieList.ItemsSource = movies;
                                MovieList.SelectedIndex = 0;
                            }
                            break;
                        case "This month":
                            movies = GetMonthPopular();

                            if (movies.Count() == 0)
                            {
                                MessageBox.Show("No orders have been made this month");
                            }
                            else
                            {
                                MovieList.DisplayMemberPath = "Title";
                                MovieList.ItemsSource = movies;
                                MovieList.SelectedIndex = 0;
                            }
                            break;
                        case "This year":
                            movies = GetYearPopular();

                            if (movies.Count() == 0)
                            {
                                MessageBox.Show("No orders have been made this year");
                            }
                            else
                            {
                                MovieList.DisplayMemberPath = "Title";
                                MovieList.ItemsSource = movies;
                                MovieList.SelectedIndex = 0;
                            }
                            break;
                    }

                    break;
            }
        }

        private List<Movie> SearchByTitle(string title)
        {
            List<Movie> movies = new List<Movie>();

            using (var context = new MovieRentalEntities())
            {
                movies = context.Movies.Where(movie => movie.Title.Contains(SearchBox.Text)).ToList();
                return movies;
            }
        }

        private List<Movie> SearchByGenre(string genre)
        {
            List<Movie> movies = new List<Movie>();

            using (var context = new MovieRentalEntities())
            {
                movies = context.Movies.Where(movie => movie.Genre.Equals(genre)).ToList();
                return movies;
            }
        }

        // Search movies by actor name
        private List<Movie> SearchByActor(string name)
        {
            List<Movie> movies = new List<Movie>();
            List<Actor> actors = new List<Actor>();

            // User entered both first and last name
            if (name.Contains(" "))
            {
                string first; string last;
                string[] names = name.Split(' ');

                first = names[0];
                last = names[1];

                using (var context = new MovieRentalEntities())
                {
                    var firstSearch = context.Actors.Where(a => a.FirstName.Equals(first)).ToList();
                    var lastSearch = context.Actors.Where(a => a.LastName.Equals(last)).ToList();

                    actors.AddRange(firstSearch);
                    actors.AddRange(lastSearch);

                    var unique = new HashSet<Actor>(actors);

                    foreach (Actor actor in unique)
                    {
                        string fullName = actor.FirstName + " " + actor.LastName;

                        // Rebuild the full name and check if it is equal to the searched name
                        if (fullName == name)
                        {
                            var actorCredits = context.Credits.Where(credit => credit.ActorID == actor.ActorID).ToList();

                            // Add every movie the actor has been in to the result set
                            foreach (Credit credit in actorCredits)
                            {
                                var movie = context.Movies.Where(m => m.MovieID == credit.MovieID).FirstOrDefault();
                                movies.Add(movie);
                            }
                        }

                    }
                }
            }
            else
            {
                // User entered either a first name or a last name
                using (var context = new MovieRentalEntities())
                {
                    // E.g - User entered "Chris", find all Chris/Christopher/Christian/etc actors
                    var firstSearch = context.Actors.Where(a => a.FirstName.Contains(name)).ToList();

                    // E.g - User entered "Pratt", find all actors with last name Pratt or something containing Pratt
                    var lastSearch = context.Actors.Where(a => a.LastName.Contains(name)).ToList();

                    // Add all matches to the list
                    actors.AddRange(firstSearch);
                    actors.AddRange(lastSearch);

                    var unique = new HashSet<Actor>(actors);

                    foreach (Actor actor in unique)
                    {
                        var actorCredits = context.Credits.Where(credit => credit.ActorID == actor.ActorID).ToList();

                        // Add every movie the actor has been in to the result set
                        foreach (Credit credit in actorCredits)
                        {
                            var movie = context.Movies.Where(m => m.MovieID == credit.MovieID).FirstOrDefault();
                            movies.Add(movie);
                        }
                    }
                }
            }
         
            return movies;
        }

        private void SearchBy_DropDownClosed(object sender, EventArgs e)
        {
            string current = (string)SearchBy.SelectedItem;

            switch (current)
            {
                case "Title":
                    SearchBox.Visibility = Visibility.Visible;
                    Genres.Visibility = Visibility.Hidden;
                    Timespan.Visibility = Visibility.Hidden;
                    break;
                case "Actor":
                    SearchBox.Visibility = Visibility.Visible;
                    Genres.Visibility = Visibility.Hidden;
                    Timespan.Visibility = Visibility.Hidden;
                    break;
                case "Genre":
                    Genres.Visibility = Visibility.Visible;
                    SearchBox.Visibility = Visibility.Hidden;
                    Timespan.Visibility = Visibility.Hidden;
                    break;
                case "Most Popular":
                    Timespan.Visibility = Visibility.Visible;
                    Genres.Visibility = Visibility.Hidden;
                    SearchBox.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private List<Movie> GetWeekPopular()
        {
            List<Movie> movies = new List<Movie>();

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
                var weekMovies = "SELECT MovieID, COUNT(OrderID) as NumberOfOrders FROM dbo.Orders WHERE Orders.RentalDate >= @date GROUP BY MovieID ORDER BY (NumberOfOrders) DESC";
                var popularThisWeek = context.Database.SqlQuery<StorePopular>(weekMovies, new SqlParameter("@date", date.ToShortDateString()));

                var top10 = popularThisWeek.Take(10); // 10 most popular movies of the week

                foreach (StorePopular popular in top10)
                {
                    Movie movie = context.Movies.Where(m => m.MovieID == popular.MovieID).Single();
                    movies.Add(movie); 
                }
            }

            return movies;
        }

        private List<Movie> GetMonthPopular()
        {
            List<Movie> movies = new List<Movie>();

            DateTime firstOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            using (var context = new MovieRentalEntities())
            {
                var monthMovies = "SELECT MovieID, COUNT(OrderID) as NumberOfOrders FROM dbo.Orders WHERE Orders.RentalDate >= @date GROUP BY MovieID ORDER BY (NumberOfOrders) DESC";
                var popularThisMonth = context.Database.SqlQuery<StorePopular>(monthMovies, new SqlParameter("@date", firstOfMonth.ToShortDateString()));

                var top10 = popularThisMonth.Take(10); // 10 most popular movies of the month

                foreach (StorePopular popular in top10)
                {
                    Movie movie = context.Movies.Where(m => m.MovieID == popular.MovieID).Single();
                    movies.Add(movie);
                }
            }

            return movies;
        }

        private List<Movie> GetYearPopular()
        {
            List<Movie> movies = new List<Movie>();

            DateTime firstOfYear = new DateTime(DateTime.Now.Year, 1, 1);

            using (var context = new MovieRentalEntities())
            {
                var yearMovies = "SELECT MovieID, COUNT(OrderID) as NumberOfOrders FROM dbo.Orders WHERE Orders.RentalDate >= @date GROUP BY MovieID ORDER BY (NumberOfOrders) DESC";
                var popularThisYear = context.Database.SqlQuery<StorePopular>(yearMovies, new SqlParameter("@date", firstOfYear.ToShortDateString()));

                var top10 = popularThisYear.Take(10); // 10 most popular movies of the year

                foreach (StorePopular popular in top10)
                {
                    Movie movie = context.Movies.Where(m => m.MovieID == popular.MovieID).Single();
                    movies.Add(movie);
                }
            }

            return movies;
        }
    }
}
