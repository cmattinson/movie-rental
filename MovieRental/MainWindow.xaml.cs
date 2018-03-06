using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TMDbLib.Objects.Collections;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using APIMovie = TMDbLib.Objects.Movies.Movie;

namespace MovieRental
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TMDbClient client = new TMDbClient("ce183dd9fdee061774d69813580b16ea");
        public MainWindow()
        {
            InitializeComponent();

            MovieListBox.SelectedIndex = 0;

            // Initially populate the list with the most popular movies at the time
            SearchContainer<SearchMovie> initial = client.GetMoviePopularListAsync("English").Result;

            List<SearchMovie> popular = initial.Results.ToList();
            MovieListBox.DisplayMemberPath = "Title";

            MovieListBox.ItemsSource = popular;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchContainer<SearchMovie> results = client.SearchMovieAsync(MovieSearchBox.Text).Result;

            List<SearchMovie> movies = results.Results.ToList();
            MovieListBox.DisplayMemberPath = "Title";

            MovieListBox.ItemsSource = movies;
        }

        // Update the poster on selection changed
        private void MovieListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SearchMovie current = (SearchMovie)MovieListBox.SelectedItem;

                Uri apiUri = new Uri("http://image.tmdb.org/t/p/w342//");
                string posterPath = current.PosterPath;

                System.UriBuilder uriBuilder = new System.UriBuilder(apiUri);
                uriBuilder.Path += posterPath;

                // TODO: Check if this is null
                MoviePoster.Source = new BitmapImage(uriBuilder.Uri);

                MovieTitle.Text = current.Title;
                MovieOverview.Text = current.Overview;
               
            }
            catch (NullReferenceException)
            {

            }

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var context = new MovieRentalEntities();

            SearchMovie current = (SearchMovie)MovieListBox.SelectedItem;

            // TODO: Move this
            var genreDict = new Dictionary<int, string>();

            genreDict.Add(28, "Action");
            genreDict.Add(12, "Adventure");
            genreDict.Add(16, "Animation");
            genreDict.Add(35, "Comedy");
            genreDict.Add(80, "Crime");
            genreDict.Add(99, "Documentary");
            genreDict.Add(18, "Drama");
            genreDict.Add(10751, "Family");
            genreDict.Add(14, "Fantasy");
            genreDict.Add(36, "History");
            genreDict.Add(27, "Horror");
            genreDict.Add(10402, "Music");
            genreDict.Add(9648, "Mystery");
            genreDict.Add(10749, "Romance");
            genreDict.Add(878, "Science Fiction");
            genreDict.Add(10770, "TV Movie");
            genreDict.Add(53, "Thriller");
            genreDict.Add(10752, "War");
            genreDict.Add(37, "Western");

            var image = client.GetMovieImagesAsync(current.Id);
            Console.WriteLine(image);

            Movie movie = new Movie()
            {
                MovieID = current.Id,
                Title = current.Title,
                Genre = genreDict[current.GenreIds[0]], // First available genre
                DistributionFee = 20000,
                NumberOfCopies = 10,
                Rating = (int) Math.Round (current.VoteAverage)
            };

            try
            {
                context.Movies.Add(movie);
                context.SaveChanges();
            } 
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                MessageBox.Show("This movie is already in the database");
            }
        }
    }
}
