// Students CRUD controller
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public StudentsController(SchoolDbContext context)
        {
            _context = context;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Student model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _context.Schools.AnyAsync(s => s.Id == model.SchoolId))
                return BadRequest("School does not exist");

            if (await _context.Students.AnyAsync(s => s.Email == model.Email))
                return BadRequest("Email already exists");

            if (await _context.Students.AnyAsync(s => s.StudentCode == model.StudentCode))
                return BadRequest("StudentCode already exists");

            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            _context.Students.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // READ + PAGINATION
        [HttpGet]
        public async Task<IActionResult> Get(int page = 1)
        {
            page = Math.Max(1, page);
            const int pageSize = 10;

            var total = await _context.Students.CountAsync();
            var students = await _context.Students
                .Include(s => s.School)
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { Page = page, PageSize = pageSize, Total = total, Items = students });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _context.Students
                .Include(s => s.School)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound("Student not found");

            return Ok(student);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Student model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound("Student not found");

            if (!await _context.Schools.AnyAsync(s => s.Id == model.SchoolId))
                return BadRequest("School does not exist");

            if (await _context.Students.AnyAsync(s => s.Email == model.Email && s.Id != id))
                return BadRequest("Email already exists");

            if (await _context.Students.AnyAsync(s => s.StudentCode == model.StudentCode && s.Id != id))
                return BadRequest("StudentCode already exists");

            student.FullName = model.FullName;
            student.Email = model.Email;
            student.Phone = model.Phone;
            student.SchoolId = model.SchoolId;
            student.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok("Student updated successfully");
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound("Student not found");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}