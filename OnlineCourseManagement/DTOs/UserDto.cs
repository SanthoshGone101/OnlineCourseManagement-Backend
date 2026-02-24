using System.ComponentModel.DataAnnotations;

namespace OnlineCourseManagement.DTOs
{
    public class UserDto 
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } 
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")] 
        public string Email { get; set; } 
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        public string Password { get; set; } 
        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|Student)$", ErrorMessage = "Role must be either Admin or Student")]
        public string Role { get; set; } }
}
