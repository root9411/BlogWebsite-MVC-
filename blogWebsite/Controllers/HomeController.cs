using blogWebsite.Data;
using blogWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace blogWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext db;

        public HomeController(AppDbContext _db) 
        {
            db = _db;
        }

        public IActionResult Index()
        {
            SharedLayoutData();
            IEnumerable<Post> myPost = db.Tbl_Post;
            return View(myPost);
        }


        [Route("Home/Post/{Slug}")]
        public IActionResult Post(string Slug)
        {
            SharedLayoutData();
            var DetailedPost = db.Tbl_Post.Where(x => x.Slug==Slug).FirstOrDefault();
            return View(DetailedPost);
        }


        public void SharedLayoutData()
        {
            ViewBag.Post = db.Tbl_Post;
            ViewBag.Profile = db.Tbl_Profile.FirstOrDefault();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}