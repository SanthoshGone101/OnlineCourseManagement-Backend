namespace OnlineCourseManagement.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin / Student public ICollection<Enrollment> Enrollments { get; set; } }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Enrollment>? Enrollments { get; set; }

    }
}

