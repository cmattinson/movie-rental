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

namespace MovieRental.Managers
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        Movie movie;

        public EditWindow(Movie movie)
        {
            InitializeComponent();

            this.movie = movie;

            Title.Text = movie.Title;

            Genre.ItemsSource = GenreDict.genreDict;
            Genre.SelectedValuePath = "Value";
            Genre.DisplayMemberPath = "Value";

            Genre.SelectedValue = movie.Genre;

            DistributionFee.Text = movie.DistributionFee.ToString("0.00");
            NumberOfCopies.Text = movie.NumberOfCopies.ToString();

            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int copies; decimal fee;

            string genre = Genre.SelectedValue.ToString();

            try
            {
                copies = Convert.ToInt32(NumberOfCopies.Text);
            }
            catch
            {
                MessageBox.Show("Error in number of copies");
                return;
            }

            try
            {
                fee = Convert.ToDecimal(DistributionFee.Text);
            }
            catch
            {
                MessageBox.Show("Error in distribution fee");
                return;
            }

            using (var context = new MovieRentalEntities())
            {
                var movie = context.Movies.Where(m => m.MovieID == this.movie.MovieID).Single();

                movie.Genre = genre;
                movie.NumberOfCopies = copies;
                movie.DistributionFee = fee;

                context.SaveChanges();

                if (MessageBox.Show("Changes saved", movie.Title, MessageBoxButton.OK, MessageBoxImage.None) == MessageBoxResult.OK)
                {
                    this.Close();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
