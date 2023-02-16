using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Data;
using westcoast_education.api.Models;
using westcoast_education.api.ViewModel;
using westcoast_education.api.ViewModel.Course;

namespace westcoast_education.api.Controllers
{
    [ApiController]
    [Route("api/v1/courses")]
   
    public class CoursesController : ControllerBase
    {
        private readonly WestcoastEducationContext _context;
        
        public CoursesController(WestcoastEducationContext context)
        {
            
            _context = context;
        }

        //* LISTA ALLA KURSER...        
        [HttpGet("listall")]
        public async Task<IActionResult> ListAll()
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
        public async Task<ActionResult> GetByTitle(string title)
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
            .SingleOrDefaultAsync(v => v.Title == title);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //* HÄMTAR KURS EFTER STARTDATUM...
        [HttpGet("startdate/{startdate}")]
        public async Task<ActionResult> GetByStartDate(DateTime startdate)
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
            .SingleOrDefaultAsync(v => v.StartDate == startdate);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //* LÄGG TILL KURS I SYSTEMET....
        [HttpPost()]
        public async Task<IActionResult> AddCourse(AddCourseViewModel model)
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
                var result = new CourseDetailViewModel
                {
                    CourseId = course.CourseId,
                    Title = course.Title,
                    StartDate = course.StartDate,
                    EndDate = course.EndDate
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
            // Hämta kursen från databasen med det angivna id:et...
            var course = await _context.Courses.FindAsync(id);
            // Kontrollera om det finns en kurs i databasen med det angivna id:et..
            if (course is null) return BadRequest($"Vi kan inte hitta en kurs i systemet med {model.CourseNumber}");
            // Uppdatera kursinformationen med den nya informationen som har skickats in från klienten..
            course.CourseNumber = model.CourseNumber;
            course.Title = model.Title;
            course.Duration = model.Duration;
            course.StartDate = model.StartDate;

            // Uppdatera databasen med den uppdaterade kursen ...      
            _context.Courses.Update(course);
            // Kontrollera om det gick att spara ändringarna till databasen och skicka sedan en NoContent response...
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return StatusCode(500, "Internal Server Error");
        }

        //* MARKERA KURS SOM FULL...
        [HttpPatch("{id}")]
        public async Task<IActionResult> MarkAsFull(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course is null) return BadRequest($"Vi kan inte hitta en kurs i systemet med {id}");
            // Uppdatera kursens status till Full...
            course.Status = CourseStatusEnum.Full;

            _context.Courses.Update(course);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return StatusCode(500, "Internal Server Error");
        }

        //* MARKERA KURS SOM KLAR...
        [HttpPatch("markasdone/{id}")]
        public async Task<ActionResult> MarkAsDone(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course is null) return BadRequest($"Vi kan inte hitta en kurs i systemet med {id}");

            // Uppdatera kursens status till Completed...
            course.Status = CourseStatusEnum.Completed;

            _context.Courses.Update(course);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
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