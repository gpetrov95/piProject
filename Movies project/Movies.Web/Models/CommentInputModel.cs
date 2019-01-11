using Movies.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Events.Web.Models
{
    public class CommentInputModel
    {
        [MaxLength(200)]
        public string Text { get; set; }
        public virtual Movie Movie { get; set; }
    }
}