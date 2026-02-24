namespace OnlineCourseManagement.Models
{
    public class Enrollment
    { 
        public int EnrollmentId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime DateEnrolled { get; set; }
        public User? User { get; set; }
        public Course? Course { get; set; } 
    }
}
