using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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

            using (var context = new MovieRentalEntities())
            {
                var movies = from m in context.Movies select m;
                MovieList.DisplayMemberPath = "Title";

                MovieList.ItemsSource = movies.ToList();
                MovieList.SelectedIndex = 0;
            }

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
                        var actorCredits = context.Credits.Where(credit => credit.ActorID == actor.ActorID).ToList();

                        // Add every movie the actor has been in to the result set
                        foreach(Credit credit in actorCredits)
                        {
                            var movie = context.Movies.Where(m => m.MovieID == credit.MovieID).FirstOrDefault();
                            movies.Add(movie);
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
    }
}
