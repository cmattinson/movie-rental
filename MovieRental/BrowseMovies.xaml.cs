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

        public BrowseMovies()
        {
            InitializeComponent();

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

            }
            catch (NullReferenceException)
            {

            }
        }

        private void Queue_Click(object sender, RoutedEventArgs e)
        {
            Movie current = (Movie) MovieList.SelectedItem;

            using (var context = new MovieRentalEntities())
            {
                Queue queue = new Queue()
                {
                    AccountNumber = 1,
                    MovieID = current.MovieID,
                    DateAdded = System.DateTime.Today
                };

                try
                {
                    context.Queues.Add(queue);
                    context.SaveChanges();
                }
                catch (DbUpdateException error)
                {
                    
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
                    AccountNumber = 2,

                };

                try
                {
                    context.Orders.Add(order);
                    context.SaveChanges();
                }
                catch (DbUpdateException error)
                {

                }
            }
        }
    }
}
