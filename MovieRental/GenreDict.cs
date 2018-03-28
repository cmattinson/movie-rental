using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental
{
    public static class GenreDict
    {
        public static Dictionary<int, string> genreDict;

        static GenreDict()
        {
            genreDict = new Dictionary<int, string>()
                { { 28, "Action" } };
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
        }

    }
}

