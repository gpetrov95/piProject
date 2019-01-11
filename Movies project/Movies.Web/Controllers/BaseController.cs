namespace Movies.Web.Controllers
{
    using System.Web.Mvc;

    using Movies.Data;

    using Microsoft.AspNet.Identity;

    [ValidateInput(false)]
    public abstract class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        public bool IsAdmin()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = (currentUserId != null && this.User.IsInRole("Administrator"));
            return isAdmin;
        }
    }
}
