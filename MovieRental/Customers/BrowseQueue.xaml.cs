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

            var gridView = new GridView();
            this.Queue.View = gridView;

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Movie ID",
                DisplayMemberBinding = new Binding("MovieID")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Title",
                DisplayMemberBinding = new Binding("Title")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Genre",
                DisplayMemberBinding = new Binding("Genre")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Rating",
                DisplayMemberBinding = new Binding("Rating")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Date Added",
                DisplayMemberBinding = new Binding("Date")
            });

            using (var context = new MovieRentalEntities())
            {
                var queues = context.Queues.Where(q => q.AccountNumber == customer.AccountNumber).ToList();

                foreach (Queue queue in queues)
                {
                    Queue.Items.Add(new QueueView
                    {
                        MovieID = queue.MovieID,
                        Title = context.Movies.Where(m => m.MovieID == queue.MovieID).Select(m => m.Title).FirstOrDefault(),
                        Genre = context.Movies.Where(m => m.MovieID == queue.MovieID).Select(m => m.Genre).FirstOrDefault(),
                        Rating = context.Movies.Where(m => m.MovieID == queue.MovieID).Select(m => m.Rating).FirstOrDefault(),
                        Date = queue.DateAdded.ToShortDateString()
                    });
                }
            }
        }
    }
}
