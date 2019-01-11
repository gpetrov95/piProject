namespace Movies.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Comment
    {
        public Comment()
        {
            this.Date = DateTime.Now;    
        }

        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }
     
        public int MovieId { get; set; }

        [Required]
        public virtual Movie Movie { get; set; }

        
    }
}
