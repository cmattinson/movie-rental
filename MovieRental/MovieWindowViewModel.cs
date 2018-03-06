using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace MovieRental
{
    class MovieWindowViewModel : INotifyPropertyChanged
    {
        MovieRentalEntities context = new MovieRentalEntities();
        private List<Movie> _movies;
        
        
        public MovieWindowViewModel()
        {

        }

        private void FillMovies()
        {
            var q = (from movie in context.Movies select movie).ToList();

            this.Movies = q;
        }

        public List<Movie> Movies
        {
            get
            {
                return _movies;
            }
            set
            {
                _movies = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
