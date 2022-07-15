using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Testapi.DataTranserObjects;
using Testapi.Models;
using AutoMapper;

namespace Testapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly PeopleContext _context;
        private static Mapper Mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Person, PersonDTO>()));
        private static Mapper InverseMapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<PersonDTO, Person>()));
        private readonly IMapper _mapper;

        public PeopleController(PeopleContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonDTO>>> GetPeople()
        {
            if (_context.People == null)
            {
                return NotFound();
            }
            // return await _context.People.ToListAsync();
            return await _context.People.Select(x => PersonToDTO(x))
                .ToListAsync();
        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDTO>> GetPerson(int id)
        {
            if (_context.People == null)
            {
                return NotFound();
            }
            var person = await _context.People.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return PersonToDTO(person);
        }

        // PUT: api/People/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, PersonDTO personDTO)
        {
            if (id != personDTO.Id)
            {
                return BadRequest();
            }

            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            var PersonUpdated = InverseMapper.Map<Person>(personDTO);
            _context.Entry(person).State = EntityState.Detached;
            _context.Entry(PersonUpdated).State = EntityState.Modified; // This throws an exception if I don't deatach the previous instance

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/People
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PersonDTO>> PostPerson(PersonDTO personDTO)
        {
            var person = InverseMapper.Map<Person>(personDTO);
            // var person = new Person
            // {
            //     Id = personDTO.Id,
            //     Name = personDTO.Name,
            //     Adresses = personDTO.Adresses
            // };

            if (_context.People == null)
            {
                return Problem("Entity set 'PeopleContext.People'  is null.");
            }
            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, PersonToDTO(person));
        }

        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            if (_context.People == null)
            {
                return NotFound();
            }
            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.People.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(int id)
        {
            return (_context.People?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static PersonDTO PersonToDTO(Person todoItem) => Mapper.Map<PersonDTO>(todoItem);
    }
}
