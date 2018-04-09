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
using TMDbLib.Client;

namespace MovieRental.Customers
{
    /// <summary>
    /// Interaction logic for RatingWindow.xaml
    /// </summary>
    public partial class RatingWindow : Window
    {
        Movie movie;
        // Connect to api using api key
        TMDbClient client = new TMDbClient("ce183dd9fdee061774d69813580b16ea");

        public RatingWindow(Movie movie)
        {
            InitializeComponent();

            this.movie = movie;
            Title.Text = movie.Title;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MovieRentalEntities())
            {
                var mov = context.Movies.Where(m => m.MovieID == this.movie.MovieID).Single();

                int total = (int) client.GetMovieAsync(mov.MovieID).Result.VoteAverage * client.GetMovieAsync(mov.MovieID).Result.VoteCount;
                int votes = client.GetMovieAsync(mov.MovieID).Result.VoteCount;

                votes = votes++;

                if (RateOne.IsChecked == true)
                {
                    total = total + 1;
                }
                else if (RateTwo.IsChecked == true)
                {
                    total = total + 2;
                }
                else if (RateThree.IsChecked == true)
                {
                    total = total + 3;
                }
                else if (RateFour.IsChecked == true)
                {
                    total = total + 4;
                }
                else if (RateFive.IsChecked == true)
                {
                    total = total + 5;
                }

                Console.WriteLine(total); Console.WriteLine(votes);

                mov.Rating = (int)Math.Round((float)(total / votes)) / 2;

                context.SaveChanges();

                this.Close();
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
