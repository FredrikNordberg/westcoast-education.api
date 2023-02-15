using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Data;
using westcoast_education.api.Models;
using westcoast_education.api.ViewModel;
using westcoast_education.api.ViewModel.Skill;

namespace westcoast_education.api.Controllers
{
    [ApiController]
    [Route("api/v1/skills")]
    public class SkillsController : ControllerBase
    {
        private readonly WestcoastEducationContext _context;
        public SkillsController(WestcoastEducationContext context)
        {
            _context = context;
        }

        //* LISTA ALLA KOMPETENSER...
        [HttpGet("listall")]
        public async Task<IActionResult> ListAll()
        {
            var result = await _context.Skill
            .Select(s => new SkillListViewModel
            {
                Id = s.Id,
                SkillName = s.SkillName
            })
            .ToListAsync();
            return Ok(result);
        }

        
        //* LÄGG TILL NY KOMPETENS... 
        [HttpPost]
        public async Task<ActionResult> AddSkill(AddSkillViewModel model)
        {
            // Kontrollera om modellen som skickats in är giltig...
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Skapa en ny instans av Skill med hjälp av data från AddSkillViewModel...
            var skill = new Skill
            {
                Id = Guid.NewGuid(),
                SkillName = model.SkillName
            };

            _context.Skill.Add(skill);
            await _context.SaveChangesAsync();

            var skillViewModel = new SkillViewModel
            {
                Id = skill.Id,
                SkillName = skill.SkillName
            };

            return CreatedAtAction(nameof(AddSkill), new { id = skill.Id }, skillViewModel);
        }


        //* UPPDATERA KOMPETENS....
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSkill(Guid id, UpdateSkillViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Information saknas för att kunna uppdatera kompetensen");

            var skill = await _context.Skill.FindAsync(id);

            if (skill is null) return BadRequest($"Vi kan inte hitta en kompetens i systemet med {model.SkillName}");

            skill.SkillName = model.SkillName;

            _context.Skill.Update(skill);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return StatusCode(500, "Internal Server Error");
        }


        //* TA BORT KOMPETENS...
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSkill(Guid id)
        {
            var skill = await _context.Skill.FindAsync(id);

            if (skill is null) return NotFound($"Vi kan inte hitta någon kompetens med id: {id}");

            _context.Skill.Remove(skill);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return StatusCode(500, "Internal Server Error");
        }
    }
}