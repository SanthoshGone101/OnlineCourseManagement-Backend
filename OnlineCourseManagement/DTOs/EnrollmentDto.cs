using System.ComponentModel.DataAnnotations;

namespace OnlineCourseManagement.DTOs
{
    public class EnrollmentDto
    {
        public int EnrollmentId { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "CourseId is required")]
        public int CourseId { get; set; }
        public DateTime DateEnrolled { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; } 
        public string CourseTitle { get; set; }
    }
}
