using blogWebsite.Data;
using blogWebsite.Models;
using blogWebsite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace blogWebsite.Controllers
{
    public class AdminController : Controller
    {

        AppDbContext db;
        IWebHostEnvironment env;
        public AdminController(AppDbContext _db, IWebHostEnvironment environment)
        {
            db = _db;
            env = environment;
        }

        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("LoginFlag") !=null)
            {
                ViewBag.NumberOfPost = db.Tbl_Post.Count();
                ViewBag.NumberOfUsers = db.Tbl_Profile.Count();
                DisplayData();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

        public IActionResult AddPost()
        {
            if (HttpContext.Session.GetString("LoginFlag") != null)
            {
                DisplayData();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }


        [HttpPost]
        public IActionResult AddPost(PostVM myPost)
        {
            if (HttpContext.Session.GetString("LoginFlag") != null)
            {
                DisplayData();
            if (ModelState.IsValid)
            {
                string ImageName = myPost.Image.FileName.ToString();
                var FolderPath = Path.Combine(env.WebRootPath, "Images");
                var CompletePicPath = Path.Combine(FolderPath, ImageName);
                myPost.Image.CopyTo(new FileStream(CompletePicPath, FileMode.Create));

                Post post = new Post();
                post.Title = myPost.Title;
                post.SubTitle = myPost.SubTitle;
                post.Date = myPost.Date;
                post.Slug = myPost.Slug;
                post.Content = myPost.Content;
                post.Image = ImageName;
                db.Tbl_Post.Add(post);
                db.SaveChanges();

                return RedirectToAction("AllPost", "Admin");


            }
            return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public IActionResult AllPost()
        {
            if (HttpContext.Session.GetString("LoginFlag") != null)
            {
                DisplayData();
                var myAllPost = db.Tbl_Post;
                return View(myAllPost);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

 
        public IActionResult DeletePost(int Id)
        {
            var PostToDelete = db.Tbl_Post.Find(Id);
            if(PostToDelete != null)
            {
                db.Remove(PostToDelete);
                db.SaveChanges();
            }
            else
            {
                Console.WriteLine("No Data Found");
            }
            return RedirectToAction("AllPost", "Admin");
        }

        public IActionResult updatePost(int Id)
        { 
            if (HttpContext.Session.GetString("LoginFlag") != null)
            {
                DisplayData();
                var PostToUpdate = db.Tbl_Post.Find(Id);
                return View(PostToUpdate);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public IActionResult updatePost(Post post)
        {
            db.Tbl_Post.Update(post);
            db.SaveChanges();
            return RedirectToAction("AllPost", "Admin");
        }

        public IActionResult CreateProfile()
        {
            if (HttpContext.Session.GetString("LoginFlag") != null)
            {
                DisplayData();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public IActionResult CreateProfile(ProfileVM profileVM)
        {
            if (HttpContext.Session.GetString("LoginFlag") != null)
            {
                DisplayData();
                if (ModelState.IsValid)
                {
                    string ImageName = profileVM.Image.FileName.ToString();
                    var FolderPath = Path.Combine(env.WebRootPath, "Images");
                    var CompleteImagePath = Path.Combine(FolderPath, ImageName);
                    profileVM.Image.CopyTo(new FileStream(CompleteImagePath, FileMode.Create));

                    Profile profile = new Profile();
                    profile.Name = profileVM.Name;
                    profile.FatherName = profileVM.FatherName;
                    profile.Bio = profileVM.Bio;
                    profile.Image = ImageName;
                    profile.username = profileVM.username;
                    profile.Password = profileVM.Password;

                    db.Tbl_Profile.Add(profile);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Admin");
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVM)
        {
            if(ModelState.IsValid)
            {
                var result = db.Tbl_Profile.Where(opt => opt.username.Equals(loginVM.Username) && opt.Password.Equals(loginVM.Password)).FirstOrDefault();
                if(result != null)
                {
                    HttpContext.Session.SetInt32("ProfileId",result.Id);
                    HttpContext.Session.SetString("LoginFlag", "true");
                    return RedirectToAction("Index", "Admin");
                }
                ViewData["LoginFlag"] = "Invalid username or Password";
                return View();
            }
            return View(new LoginVM());
        }

        public void DisplayData()
        {
            ViewBag.Profile = db.Tbl_Profile.Where(x => x.Id.Equals(HttpContext.Session.GetInt32("ProfileId"))).AsNoTracking().FirstOrDefault();

        }

        public IActionResult UpdateProfile(int id)
        {
            if (HttpContext.Session.GetString("LoginFlag") != null)
            {
                DisplayData();

                var myProfile = db.Tbl_Profile.Find(id);
                ProfileVM pvm = new ProfileVM();
                pvm.Id = myProfile.Id;
                pvm.Name = myProfile.Name;
                pvm.FatherName = myProfile.FatherName;
                pvm.Bio = myProfile.Bio;
                pvm.username = myProfile.username;
                pvm.Password = myProfile.Password;
                pvm.ConfirmPassword = myProfile.Password;
                ViewData["ImageName"] = myProfile.Image;


                return View(pvm);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public IActionResult UpdateProfile(ProfileVM myProfile,string? oldPic)
        {
            DisplayData();
            string imageName = null;
            if (ModelState.IsValid)
            {
                if(myProfile.Image!= null)
                {
                    imageName = myProfile.Image.FileName.ToString();
                    var FolderPath = Path.Combine(env.WebRootPath, "Images");
                    var ImagePath = Path.Combine(FolderPath, imageName);
                    myProfile.Image.CopyTo(new FileStream(ImagePath, FileMode.Create));
                }

                Profile originalProfile = new Profile();
                originalProfile.Id = myProfile.Id;
                originalProfile.Name = myProfile.Name;
                originalProfile.FatherName = myProfile.FatherName;
                originalProfile.Bio = myProfile.Bio;
                originalProfile.Password = myProfile.ConfirmPassword;


                if(!string.IsNullOrEmpty(imageName))
                {
                    originalProfile.Image = imageName;
                }
                else
                {
                    originalProfile.Image = oldPic;
                }
                db.Tbl_Profile.Update(originalProfile);
                db.SaveChanges();
                return RedirectToAction("Index","Admin");

            }

            if (!string.IsNullOrEmpty(imageName))
            {

                ViewData["ImageName"] = imageName;
            }
            else
            {
                ViewData["ImageName"] = oldPic;
            }
            
            return View();
        }

    }
}
