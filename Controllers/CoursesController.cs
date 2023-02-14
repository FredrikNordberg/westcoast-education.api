using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wescoast_education.api.ViewModels.Courses;
using westcoast_education.api.Data;
using westcoast_education.api.Models;
using westcoast_education.api.ViewModel;

namespace westcoast_education.api.Controllers
{
     [ApiController]
    [Route("api/v1/courses")]
    [Produces("application/json")]
    public class CoursesController : ControllerBase
    {
        private readonly WestcoastEducationContext _context;
        public CoursesController(WestcoastEducationContext context)
        {
            _context = context;
        }

        //* LISTA ALLA KURSER...        
        [HttpGet("listall")]
        public async Task<IActionResult> ListAllCourses()
        {
            var result = await _context.Courses
            .Select(c => new
            {
                CourseId = c.CourseId,
                Title = c.Title,
                StartDate = c.StartDate.ToShortDateString(),
                Teacher = c.Teacher != null ? c.Teacher.FirstName + " " + c.Teacher.LastName : "Saknas",
                Students = c.StudentCourses.Select(s => new
                {
                    StudentId = s.StudentId,
                    Name = s.Student.FirstName + " " + s.Student.LastName
                }).ToList()
            })
            .ToListAsync();

            return Ok(result);
        }

        //* HÄMTA KURS GENOM ID:t.... 
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _context.Courses
                .Select(c => new
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    StartDate = c.StartDate.ToShortDateString(),
                    EndDate = c.EndDate.ToShortDateString(),
                    Teacher = c.Teacher != null ? new
                    {
                        Id = c.Teacher.Id,
                        FirstName = c.Teacher.FirstName,
                        LastName = c.Teacher.LastName,
                        Email = c.Teacher.Email,
                        Phone = c.Teacher.Phone
                    } : null,
                    Students = c.StudentCourses.Select(s => new
                    {
                        Id = s.Student.Id,
                        FirstName = s.Student.FirstName,
                        LastName = s.Student.LastName,
                        Email = s.Student.Email,
                        Phone = s.Student.Phone,
                        Status = ((CourseStatusEnum)s.Status).ToString()
                    })
                })
                .SingleOrDefaultAsync(c => c.CourseId == id);

            return Ok(result);
        }

        //* LÄGG TILL KURS I SYSTEMET....
        [HttpPost()]
        public async Task<IActionResult> AddCourse(CoursesPostViewModel model)
        {
            var exists = await _context.Courses.SingleOrDefaultAsync(
                c => c.CourseNumber == model.CourseNumber &&
                c.StartDate == model.StartDate);

            if (exists is not null) return BadRequest($"Kurs med kursnummer {model.CourseNumber} och kurs start {model.StartDate.ToShortDateString()} existerar redan");

            var course = new Course
            {
                CourseId = Guid.NewGuid(),
                Title = model.Title,
                CourseNumber = model.CourseNumber,
                Duration = model.Duration,
                StartDate = model.StartDate,
                EndDate = model.StartDate.AddDays(model.Duration * 7)
            };

            await _context.Courses.AddAsync(course);

            if (await _context.SaveChangesAsync() > 0)
            {
                var result = new
                {
                    CourseId = course.CourseId,
                    Title = course.Title,
                    StartDate = course.StartDate.ToShortDateString(),
                    EndDate = course.EndDate.ToShortDateString()
                };
                return CreatedAtAction(nameof(GetById), new { Id = course.CourseId }, result);

            }

            return StatusCode(500, "Internal Server Error");
        }

        //* TA BORT KURS..
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course is null) return NotFound($"Vi kan inte hitta någon kurs med id: {id}");

            _context.Courses.Remove(course);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return StatusCode(500, "Internal Server Error");
        }

        

        
    }
}