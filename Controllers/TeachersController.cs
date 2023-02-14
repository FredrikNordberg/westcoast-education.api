using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wescoast_education.api.ViewModels.Courses;
using westcoast_education.api.Data;
using westcoast_education.api.Models;
using westcoast_education.api.ViewModel;

namespace westcoast_education.api.Controllers
{
    [ApiController]
    [Route("api/v1/teachers")]
    public class TeachersController : ControllerBase
    {
        private readonly WestcoastEducationContext _context;
        public TeachersController(WestcoastEducationContext context)
        {
            _context = context;
        }

        //* LISTA ALLA LÄRARE...

        [HttpGet("listall")]
        public async Task<IActionResult> ListAllTeachers()
        {
            var result = await _context.Teachers
                .Select(t => new TeacherViewModel //? Göra en TeacherListViewModel
                {
                    TeacherId = t.Id,
                    FirstName = $"{t.FirstName} {t.LastName}",
                    Email = t.Email
                })
                .ToListAsync();

            return Ok(result);
        }

        //! Anonym..
        // [HttpGet("listall")]
        // public async Task<IActionResult> ListAllTeachers()
        // {
        //     var result = await _context.Teachers
        //         .Select(t => new
        //         {
        //             TeacherId = t.Id,
        //             Name = $"{t.FirstName} {t.LastName}",
        //             Email = t.Email
        //         })
        //         .ToListAsync();

        //     return Ok(result);
        // }

        //* HÄMTAR LÄRARE GENOM ID....
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Courses)
                .Include(t => t.Skills)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            var teacherViewModel = new TeacherViewModel
            {
                TeacherId = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                Phone = teacher.Phone,
                Address = teacher.Address,
                PostalCode = teacher.PostalCode,
                City = teacher.City,
                Courses = teacher.Courses.Select(c => new CourseViewModel
                {
                    CourseId = c.CourseId,
                    Title = c.Title
                }).ToList(),
                Skills = teacher.Skills.Select(s => new SkillViewModel
                {
                    Id = s.Id,
                    SkillName = s.SkillName
                }).ToList()
            };

            return Ok(teacherViewModel);
        }





        //! Anonym..
        // [HttpGet("getbyid/{id}")]
        // public async Task<IActionResult> GetById(Guid id)
        // {
        //     var result = await _context.Teachers
        //         .Select(t => new 
        //         {
        //             TeacherId = t.Id,
        //             Name = $"{t.FirstName} {t.LastName}",
        //             Email = t.Email,
        //             Phone = t.Phone,
        //             Address = t.Address,
        //             PostalCode = t.PostalCode,
        //             City = t.City,
        //             Courses = t.Courses.Select(c => new
        //             {
        //                 CourseId = c.CourseId,
        //                 Title = c.Title
        //             }).ToList(),
        //             Skills = t.Skills.Select(s => new
        //             {
        //                 SkillId = s.Id,
        //                 Name = s.SkillName
        //             }).ToList()
        //         })
        //         .SingleOrDefaultAsync(c => c.TeacherId == id);

        //     return Ok(result);
        // }

        
        //* LÄGG TILL LÄRARE....
        [HttpPost()]
        public async Task<IActionResult> AddTeacher(TeacherPostViewModel model)
        {
            var exists = await _context.Teachers.SingleOrDefaultAsync(c => c.Email.ToLower().Trim() == model.Email.ToUpper().Trim());

            if (exists is not null) return BadRequest($"En lärare med e-post {model.Email} är redan registrerad i systemet.");

            var teacher = new Teacher
            {
                Id = Guid.NewGuid(),
                BirthOfDate = model.BirthOfDate,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                PostalCode = model.PostalCode,
                City = model.City
            };

            await _context.Teachers.AddAsync(teacher);

            foreach (var skill in model.Skills)
            {
                var s = await _context.Skill.SingleOrDefaultAsync(c => c.SkillName.ToLower().Trim() == skill.ToLower().Trim());
                if (s is null)
                {
                    s = new Skill
                    {
                        SkillName = skill.Trim()
                    };

                    await _context.Skill.AddAsync(s);
                }
                if (teacher.Skills is null) teacher.Skills = new List<Skill>();
                teacher.Skills.Add(s);
            }

            if (await _context.SaveChangesAsync() > 0)
            {
                return StatusCode(201);
            }
            return StatusCode(500, "Internal Server Error");
        }


        //* LÄGG TILL LÄRARE TILL KURS..
        [HttpPatch("addteacher")]
        public async Task<IActionResult> AddTeacherToCourse(Guid courseId, Guid teacherId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course is null) return NotFound($"Tyvärr kunde vi inte hitta någon kurs med id {courseId}");

            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher is null) return NotFound($"Tyvärr kunde vi inte hitta någon lärare med id {teacherId}");

            course.Teacher = teacher;

            _context.Update(course);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return StatusCode(500, "Internal Server Error");
        }

        //* TA BORT LÄRARE....
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacher(Guid id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher is null) return NotFound($"Vi kan inte hitta någon lärare med id: {id}");

            _context.Teachers.Remove(teacher);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return StatusCode(500, "Internal Server Error");
        }



    }
}