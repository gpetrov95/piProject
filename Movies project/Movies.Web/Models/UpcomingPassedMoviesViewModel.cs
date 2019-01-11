namespace Movies.Web.Models
{
    using System.Collections.Generic;

    public class UpcomingPassedMoviesViewModel
    {
        public IEnumerable<MovieViewModel> UpcomingMovies { get; set; }
        
        public IEnumerable<MovieViewModel> PassedMovies { get; set; }
    }
}