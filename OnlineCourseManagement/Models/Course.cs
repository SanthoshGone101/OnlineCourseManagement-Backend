namespace OnlineCourseManagement.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MediaPath { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Enrollment>? Enrollments { get; set; }

      
    }
}

