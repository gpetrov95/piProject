using System.Web.Mvc;

namespace Movies.Web.Controllers
{
    using System;
    using System.Linq;

    using Movies.Data;
    using Movies.Web.Extensions;
    using Movies.Web.Models;

    using Microsoft.AspNet.Identity;
    using Movies.Web.Models;
    using System.IO;
    using System.Web;
    using Events.Web.Models;
    using System.Data.Entity.Validation;
    using System.Data.SqlClient;

    [Authorize]
    public class MoviesController : BaseController
    {
        public ActionResult My()
        {
            string currentUserId = this.User.Identity.GetUserId();
            var movies = this.db.Movies
                 .Where(e => e.FavoriteMovie.Contains(currentUserId)) //тук ще проверява дали id- то на currentUserId го има в favoriteMovies
                //.Where(e => e.AuthorId == currentUserId)
                .OrderBy(e => e.StartDateTime)
                .Select(MovieViewModel.ViewModel);

            var upcomingMovies = movies.Where(e => e.StartDateTime > DateTime.Now);
            var passedMovies = movies.Where(e => e.StartDateTime <= DateTime.Now);
            return View(new UpcomingPassedMoviesViewModel()
            {
                UpcomingMovies = upcomingMovies,
                PassedMovies = passedMovies
            });
        }

        [HttpGet]
        public ActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MovieInputModel model, HttpPostedFileBase file) //създаване на филм и добавяне на снимка
        {

            if (file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Images"), fileName);
                //първо записвам в папката
                file.SaveAs(filePath);
                //път за записване в колона Image
                string imagePath = "~/Images/" + fileName;
                //  Console.WriteLine(imagePath);


                if (model != null && this.ModelState.IsValid)
                {
                    var e = new Movie()
                    {
                        AuthorId = this.User.Identity.GetUserId(),
                        Title = model.Title,
                        StartDateTime = model.StartDateTime,
                        Duration = model.Duration,
                        Description = model.Description,
                        Location = model.Location,
                        IsPublic = model.IsPublic,
                        Image = imagePath
                    };

                    this.db.Movies.Add(e);
                    this.db.SaveChanges();
                    this.AddNotification("Movie created.", NotificationType.INFO);
                    return this.RedirectToAction("My");
                }
            } //zatwarqm gorniq if
            return this.View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var movieToEdit = this.LoadMovie(id);
            if (movieToEdit == null)
            {
                this.AddNotification("Cannot edit movie #" + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }

            var model = MovieInputModel.CreateFromMovie(movieToEdit);
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MovieInputModel model)
        {
            var movieToEdit = this.LoadMovie(id);
            if (movieToEdit == null)
            {
                this.AddNotification("Cannot edit movie #" + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }

            if (model != null && this.ModelState.IsValid)
            {
                movieToEdit.Title = model.Title;
                movieToEdit.StartDateTime = model.StartDateTime;
                movieToEdit.Duration = model.Duration;
                movieToEdit.Description = model.Description;
                movieToEdit.Location = model.Location;
                movieToEdit.IsPublic = model.IsPublic;

                this.db.SaveChanges();
                this.AddNotification("Movie edited.", NotificationType.INFO);
                return this.RedirectToAction("My");
            }

            return this.View(model);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var movieToDelete = this.LoadMovie(id);
            if (movieToDelete == null)
            {
                this.AddNotification("Cannot delete movie #" + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }

            var model = MovieInputModel.CreateFromMovie(movieToDelete);
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, MovieInputModel model)
        {
            var movieToDelete = this.LoadMovie(id);
            if (movieToDelete == null)
            {
                this.AddNotification("Cannot delete movie #" + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }

            this.db.Movies.Remove(movieToDelete);
            this.db.SaveChanges();
            this.AddNotification("Movie deleted.", NotificationType.INFO);
            return this.RedirectToAction("My");
        }

        private Movie LoadMovie(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var movieToEdit = this.db.Movies
                .Where(e => e.Id == id)
                .FirstOrDefault(e => e.AuthorId == currentUserId || isAdmin);
            return movieToEdit;
        }

        private int GetCurrentMovie()
        {
            var currentURL = HttpContext.Request.Url.AbsolutePath;
            Console.WriteLine(currentURL);
            char[] charsToTrim = { '/', 'M', 'o', 'v', 'i', 'e', 's', '/', 'A', 'd', 'd', 'C', 'o', 'm', 'm', 'e', 'n', 't', '/' };
            string result = currentURL.Trim(charsToTrim);
            int movieID = Int32.Parse(result);
            Console.WriteLine(movieID);

            return movieID;

        }

        private Movie LoadMovieToComment(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var movieToComment = this.db.Movies
                .Where(e => e.Id == id)
                .FirstOrDefault(e => e.AuthorId != currentUserId || isAdmin);
            return movieToComment;
        }



        public ActionResult AddComment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public ActionResult AddComment(int id, CommentInputModel model)
        {

            if (model != null && this.ModelState.IsValid)
            {
                var c = new Comment()
                {
                    Movie = this.LoadMovieToComment(GetCurrentMovie()),
                    AuthorId = this.User.Identity.GetUserId(),
                    Text = model.Text

                };
                this.db.Comments.Add(c);
                try
                {

                    this.db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Console.WriteLine(e);
                }
                return this.RedirectToAction("My");
            }
            return this.View(model);
        }


        //[HttpGet]
        //public ActionResult AddToFavorite()
        //{
        //    return this.RedirectToAction("My");
        //}

       
        public ActionResult AddToFavorite(int id, MovieInputModel model)
        {
            var currentuserid = this.User.Identity.GetUserId();

            var movieToFavorite = this.LoadMovieToComment(id);
            if (movieToFavorite == null) //if cannot load movie
            {
                this.AddNotification("Cannot favorite movie #" + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }

            if (movieToFavorite.FavoriteMovie != null)
            {
                movieToFavorite.FavoriteMovie = movieToFavorite.FavoriteMovie + currentuserid; //append new user id to existing

                this.db.SaveChanges();
                this.AddNotification("Movie added to favorite.", NotificationType.INFO);
                return this.RedirectToAction("My");
            }
            else
            {
                movieToFavorite.FavoriteMovie = currentuserid;
                this.db.SaveChanges();
                this.AddNotification("Movie added to favorite.", NotificationType.INFO);
                return this.RedirectToAction("My");
            }

        }

    }

}
