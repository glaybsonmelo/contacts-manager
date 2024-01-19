using Entities;
using Microsoft.EntityFrameworkCore;
using RespositoryContracts;
using System.Linq.Expressions;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;

        public PersonsRepository(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid Id)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(person => person.Id == Id));
            int rowsDeleted = await _db.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country")
                .Where(predicate)
                .ToListAsync();

        }

        public async Task<Person?> GetPersonById(Guid id)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(person => person.Id == id);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? personToUpdate = await _db.Persons.FindAsync(person.Id);
            if (personToUpdate == null)
                return person;

            personToUpdate.Name = person.Name;
            personToUpdate.Email = person.Email;
            personToUpdate.BirthDate = person.BirthDate;
            personToUpdate.Gender = person.Gender;
            personToUpdate.CountryId = person.CountryId;
            personToUpdate.Country = person.Country;
            personToUpdate.ReceiveNewsLetters = person.ReceiveNewsLetters;
            personToUpdate.TIN = person.TIN;
            await _db.SaveChangesAsync();

            return personToUpdate;
        }
    }
}
