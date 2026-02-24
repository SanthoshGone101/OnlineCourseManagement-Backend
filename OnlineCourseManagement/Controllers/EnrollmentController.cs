using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourseManagement.Data;
using OnlineCourseManagement.DTOs;
using OnlineCourseManagement.Models;

namespace OnlineCourseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly CourseDbContext _context;

        public EnrollmentController(CourseDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Student")]
        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll([FromBody] EnrollmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if User exists
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {dto.UserId} not found.");
            }
            var existing = await _context.Enrollments
    .FirstOrDefaultAsync(e => e.UserId == dto.UserId && e.CourseId == dto.CourseId);

            if (existing != null)
            {
                return BadRequest(new { success = false, message = "Already enrolled in this course" });
            }


            // Check if Course exists
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
            {
                return NotFound($"Course with ID {dto.CourseId} not found.");
            }

            var enrollment = new Enrollment
            {
                UserId = dto.UserId,
                CourseId = dto.CourseId,
                DateEnrolled = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new EnrollmentDto
            {
                EnrollmentId = enrollment.EnrollmentId,
                UserId = enrollment.UserId,
                CourseId = enrollment.CourseId,
                DateEnrolled = enrollment.DateEnrolled,
                UserName = user.Name,
                CourseTitle = course.Title
            });
        }

        [Authorize(Roles = "Admin")]

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserEnrollments(int userId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Course).Include(e => e.User)
                .Where(e => e.UserId == userId)
                .Select(e => new EnrollmentDto
                { 
                    EnrollmentId = e.EnrollmentId, 
                    UserId = e.UserId, CourseId = e.CourseId,
                    DateEnrolled = e.DateEnrolled, 
                    UserName = e.User.Name, 
                    CourseTitle = e.Course.Title 
                })
                .ToListAsync();
            return Ok(enrollments); 
        }


        [Authorize(Roles = "Student")]
        [HttpDelete("unenroll/{courseId}")]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

            if (enrollment == null)
                return NotFound(new { success = false, message = "Enrollment not found" });

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Unenrolled successfully" });
        }
        [Authorize(Roles = "Student")]
        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);

            var courses = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .Select(e => new CourseDto
                {
                    CourseId = e.Course.CourseId,
                    Title = e.Course.Title,
                    Description = e.Course.Description
                })
                .ToListAsync();

            return Ok(new { success = true, data = courses });
        }
        [Authorize(Roles = "Instructor,Admin")]
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseEnrollments(int courseId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.User)
                .Where(e => e.CourseId == courseId)
                .Select(e => new { e.UserId, e.User.Name, e.DateEnrolled })
                .ToListAsync();

            return Ok(enrollments);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDto dto)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Course created", data = course });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound("Course not found");

            course.Title = dto.Title;
            course.Description = dto.Description;

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Course updated", data = course });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound("Course not found");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Course deleted" });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.UserId, u.Name, u.Email, u.Role })
                .ToListAsync();

            return Ok(new { success = true, data = users });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found");

            user.Role = role;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Role updated", data = user });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "User deleted" });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var totalEnrollments = await _context.Enrollments.CountAsync();

            // Optional: breakdown by role
            var studentCount = await _context.Users.CountAsync(u => u.Role == "Student");
            var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");

            return Ok(new
            {
                success = true,
                data = new
                {
                    TotalUsers = totalUsers,
                    Students = studentCount,
                    Admins = adminCount,
                    TotalCourses = totalCourses,
                    TotalEnrollments = totalEnrollments
                }
            });
        }





    }

}
