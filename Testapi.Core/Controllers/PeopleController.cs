using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Testapi.DataTranserObjects;
using Testapi.Mappings;
using Testapi.Models;
using AutoMapper;

namespace Testapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly PeopleContext _context;
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

            return await _context.People.Select(x => _mapper.Map<PersonDTO>(x))
                .ToListAsync();
        }

        // GET: api/People/addresses
        [HttpGet("addresses")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            if (_context.Addresses == null)
            {
                return NotFound();
            }
            return await _context.Addresses.ToListAsync();
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

            return _mapper.Map<PersonDTO>(person);
        }

        // GET: api/People/addresses/5
        [HttpGet("addresses/{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            if (_context.Addresses == null)
            {
                return NotFound();
            }
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // GET: api/People/5/addresses
        [HttpGet("{id}/addresses")]
        public async Task<ActionResult<IEnumerable<Address>>> GetPersonAddresses(int id)
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

            return await _context.Addresses
                .Where(x => x.PersonId.Equals(person.Id))
                .ToListAsync();
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

            var UpdatedPerson = _mapper.Map<Person>(personDTO);
            _context.Entry(person).State = EntityState.Detached;

            foreach (var address in UpdatedPerson.Addresses)
            {
                var UpdatedAddress = address;
                _context.Entry(address).State = EntityState.Detached;
                _context.Entry(UpdatedAddress).State = EntityState.Modified; // This throws an exception if I don't deatach the previous instance
            }

            _context.Entry(UpdatedPerson).State = EntityState.Modified; // This throws an exception if I don't deatach the previous instance

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
        [HttpPost]
        public async Task<ActionResult<PersonDTO>> PostPerson(PersonDTO personDTO)
        {
            var person = _mapper.Map<Person>(personDTO);

            if (_context.People == null)
            {
                return Problem("Entity set 'PeopleContext.People'  is null.");
            }
            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, _mapper.Map<PersonDTO>(person));
        }

        // POST: api/People/5
        [HttpPost("{id}")]
        public async Task<ActionResult<Address>> PostAddress(int id, Address address)
        {
            if (_context.Addresses == null)
            {
                return Problem("Entity set 'PeopleContext.Addresses'  is null.");
            }
            var person = await _context.People.FindAsync(id);
            if (person == null)
                return NotFound();
            
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
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

        // DELETE: api/People/5/2
        [HttpDelete("{PersonId}/{AddressId}")]
        public async Task<IActionResult> DeleteAddress(int PersonId, int AddressId)
        {
            if (_context.People == null || _context.Addresses == null)
            {
                return NotFound();
            }

            var Person = await _context.People.FindAsync(PersonId);
            if (Person == null)
            {
                return NotFound();
            }

            var Address = await _context.Addresses.FindAsync(AddressId);

            if (Address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(Address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(int id)
        {
            return (_context.People?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
