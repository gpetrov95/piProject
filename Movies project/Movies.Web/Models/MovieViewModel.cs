namespace Movies.Web.Models
{
    using System;
    using System.Linq.Expressions;

    using Movies.Data;

    public class MovieViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime StartDateTime { get; set; }

        public TimeSpan? Duration { get; set; }

        public string Author { get; set; }

        public string Location { get; set; }

        public string Image { get; set; }



        public static Expression<Func<Movie, MovieViewModel>> ViewModel
        {
            get
            {
                return e => new MovieViewModel()
                {
                    Id = e.Id,
                    Title = e.Title,
                    StartDateTime = e.StartDateTime,
                    Duration = e.Duration,
                    Location = e.Location,
                    Author = e.Author.FullName,
                    Image = e.Image,
                };
            }
        }
    }
}