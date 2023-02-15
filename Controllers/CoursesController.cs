using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wescoast_education.api.ViewModels.Courses;
using westcoast_education.api.Data;
using westcoast_education.api.Models;
using westcoast_education.api.ViewModel;
using westcoast_education.api.ViewModel.Course;

namespace westcoast_education.api.Controllers
{
    [ApiController]
    [Route("api/v1/courses")]
    [Produces("application/json")] //  Specificera vilken typ av svar som metoden ska generera. I detta fall anger attributet att metoden ska returnera ett JSON-svar...
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
                .Select(c => new CourseListViewModel
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    StartDate = c.StartDate,
                    // Kontrollera om kursen har en lärare, om inte , använd tom lista..
                    Teachers = c.Teacher != null
                        ? new List<TeacherDetailViewModel>
                        {
                    new TeacherDetailViewModel
                    {
                        FirstName = c.Teacher.FirstName,
                        LastName = c.Teacher.LastName
                    }
                        }
                        : new List<TeacherDetailViewModel>(),
                    // Hämta elev-data från StudentCourse-tabellen och mappa till ViewModel..
                    Students = c.StudentCourses.Select(s => new StudentDetailViewModel
                    {

                        FirstName = s.Student.FirstName,
                        LastName = s.Student.LastName,
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
            // Begäran för att hämta en kurs och projicera resultatet till en CourseDetailViewModel...
                .Select(c => new CourseDetailViewModel 
                {
                    CourseId = c.CourseId,
                    CourseNumber = c.CourseNumber,
                    Duration = c.Duration,
                    Title = c.Title,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    // Om kursen har en lärare, projicera resultatet till en TeacherDetailViewModel..
                    Teacher = c.Teacher != null ? new TeacherDetailViewModel 
                    {
                        TeacherId = c.Teacher.Id,
                        FirstName = c.Teacher.FirstName,
                        LastName = c.Teacher.LastName,
                        Email = c.Teacher.Email,
                        Phone = c.Teacher.Phone
                    } : null,
                    // Projicera resultatet av kursens StudentCourses till en lista av StudentDetailViewModels..
                    Students = c.StudentCourses.Select(s => new StudentDetailViewModel 
                    {
                        StudentId = s.Student.Id,
                        FirstName = s.Student.FirstName,
                        LastName = s.Student.LastName,
                        Email = s.Student.Email,
                        Phone = s.Student.Phone,
                        // Studentens status i kursen, som en enum..
                        Status = ((CourseStatusEnum)s.Status) 
                    }).ToList()
                })
                // Hämta endast en kurs baserat på dess unika id...
                .SingleOrDefaultAsync(c => c.CourseId == id); 

            return Ok(result);
        }


        //* HÄMTAR KURS EFTER KURSNUMMBER..
        [HttpGet("coursenumber/{coursenumber}")]
        public async Task<ActionResult> GetByCourseNumber(int coursenumber)
        {
            var result = await _context.Courses
            .Select(c => new CourseDetailViewModel
            {
                CourseId = c.CourseId,
                CourseNumber = c.CourseNumber,
                Title = c.Title,
                Duration = c.Duration,
                StartDate = c.StartDate
            })
            .SingleOrDefaultAsync(v => v.CourseNumber == coursenumber);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //* HÄMTAR KURS EFTER TITELN...
        [HttpGet("title/{title}")]
        public ActionResult GetByTitel(string titel)
        {
            return Ok(new { message = $"GetBytitel fungerar {titel}" });
        }

        //* HÄMTAR KURS EFTER STARTDATUM...
        [HttpGet("startdate/{startdate}")]
        public ActionResult GetByStartDate(string startdate)
        {
            return Ok(new { message = $"GetBytitel fungerar {startdate}" });
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

        //* UPPDATERA KURS....
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCourse(Guid id, UpdateCourseViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Information saknas för att kunna uppdater kursen");


            //* Vi måste kontrollera så att kursen inte redan är registrerad i systemet...
            var course = await _context.Courses.FindAsync(id);

            if (course is null) return BadRequest($"Vi kan inte hitta en kurs i systemet med {model.CourseNumber}");

            course.CourseNumber = model.CourseNumber;
            course.Title = model.Title;
            course.Duration = model.Duration;
            course.StartDate = model.StartDate;

            _context.Courses.Update(course);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return StatusCode(500, "Internal Server Error");
        }

        //* MARKERA KURS SOM FULL...
        [HttpPatch("{id}")]
        public ActionResult MarkAsFull(Guid id)
        {

            return NoContent();
        }

        //* MARKERA KURS SOM KLAR...
        [HttpPatch("markasdone/{id}")]
        public ActionResult MarkAsDone(Guid id)
        {

            return NoContent();
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