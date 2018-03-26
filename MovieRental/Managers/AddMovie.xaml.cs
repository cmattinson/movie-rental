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
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Search;
using APIMovie = TMDbLib.Objects.Movies.Movie; // Alias as we already have an object called Movie

namespace MovieRental
{
    /// <summary>
    /// Interaction logic for AddMovie.xaml
    /// </summary>
    public partial class AddMovie : Page
    {
        // Connect to api using api key
        TMDbClient client = new TMDbClient("ce183dd9fdee061774d69813580b16ea");

        public AddMovie()
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
                SearchMovie current = (SearchMovie) MovieListBox.SelectedItem;

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

            Credits credits = client.GetMovieCreditsAsync(current.Id).Result;

            foreach (Cast cast in credits.Cast)
            {

                // Top 5 actors
                if (cast.Order < 5)
                {
                    Person person = client.GetPersonAsync(cast.Id).Result;

                    int id = person.Id;
                    string firstName, lastName;
                    string gender = person.Gender.ToString();
                    string sex = gender[0].ToString();

                    var today = DateTime.Today;

                    int age = today.Year - person.Birthday.GetValueOrDefault().Year;

                    string fullName = person.Name;
                    var names = fullName.Split(' ');

                    // Just take first and last name if there are more than two names
                    if (names.Length > 2)
                    {
                        firstName = names[0]; lastName = names[names.Length - 1];
                    }
                    else
                    {
                        firstName = names[0]; lastName = names[1];
                    }

                    // Add the movie's top 5 actors to the actors table
                    Actor actor = new Actor()
                    {
                        ActorID = id,
                        FirstName = firstName,
                        LastName = lastName,
                        Sex = sex,
                        Age = age,
                        Rating = 1
                    };

                    try
                    {
                        context.Actors.Add(actor);
                        context.SaveChanges();
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateException)
                    {
                        // Actor already exists
                    }


                    // Add the actor's credits for this movie into the database
                    Credit credit = new Credit()
                    {
                        MovieID = current.Id,
                        ActorID = id
                    };

                    try
                    {
                        context.Credits.Add(credit);
                        context.SaveChanges();
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateException)
                    {
                        // Credit already exists
                    }
                }
            }

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
                Genre = genreDict[current.GenreIds[0]], // First available genre for the movie
                DistributionFee = 20000,
                NumberOfCopies = 10,
                Rating = (int)Math.Round(current.VoteAverage / 2)
            };

            try
            {
                context.Movies.Add(movie);
                context.SaveChanges();

                MessageBox.Show("Movie added successfully!");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                MessageBox.Show("This movie is already in the database");
            }
        }
    }
}
