namespace Movies.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Movies.Data;

    public class MovieInputModel
    {
        [Required(ErrorMessage = "Movie title is required.")]
        [StringLength(200, ErrorMessage = "The {0} must be between {2} and {1} characters long.", 
            MinimumLength = 1)]
        [Display(Name = "Title *")]
        public string Title { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date and Time *")]
        public DateTime StartDateTime { get; set; }

        public TimeSpan? Duration { get; set; }

        public string Description { get; set; }

        [MaxLength(200)]
        public string Location { get; set; }

        [Display(Name = "Is Public?")]
        public bool IsPublic { get; set; }

        public string Image { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public string FavoriteMovie { get; set; }

        public static MovieInputModel CreateFromMovie(Movie e)
        {
            return new MovieInputModel()
            {
                Title = e.Title,
                StartDateTime = e.StartDateTime,
                Duration = e.Duration,
                Location = e.Location,
                Description = e.Description,
                IsPublic = e.IsPublic
            };
        }
    }
}