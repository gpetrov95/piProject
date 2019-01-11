namespace Movies.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Movies.Web.Models;

    using Microsoft.AspNet.Identity;

    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var movies = this.db.Movies
                .OrderBy(e => e.StartDateTime)
                .Where(e => e.IsPublic)
                .Select(MovieViewModel.ViewModel);

            var upcomingMovies = movies.Where(e => e.StartDateTime > DateTime.Now);
            var passedMovies = movies.Where(e => e.StartDateTime <= DateTime.Now);
            return View(new UpcomingPassedMoviesViewModel()
            {
                UpcomingMovies = upcomingMovies,
                PassedMovies = passedMovies
            });
        }

        public ActionResult MovieDetailsById(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var movieDetails = this.db.Movies
                .Where(e => e.Id == id)
                .Where(e => e.IsPublic || isAdmin || (e.AuthorId != null && e.AuthorId == currentUserId))
                .Select(MovieDetailsViewModel.ViewModel)
                .FirstOrDefault();
            
            var isOwner = (movieDetails != null && movieDetails.AuthorId != null &&
                movieDetails.AuthorId == currentUserId);
            this.ViewBag.CanEdit = isOwner || isAdmin;

            return this.PartialView("_MovieDetails", movieDetails);
        }


    }
}
