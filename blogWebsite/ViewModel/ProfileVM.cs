using System.ComponentModel.DataAnnotations;

namespace blogWebsite.ViewModel
{
    public class ProfileVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Bio { get; set; }
        public IFormFile Image { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage ="Password And Confirm Password not match")]
        public string ConfirmPassword { get; set; } 
        public string username { get; set; }
    }
}
