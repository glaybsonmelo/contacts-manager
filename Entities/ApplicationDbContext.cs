using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<Country> Countries { get; set;}
        public virtual DbSet<Person> Persons { get; set;}
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // Seed for countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach(Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            // Seed for persons
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach(Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            //  Fluent API
            modelBuilder.Entity<Person>().Property(person => person.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABCD1234");

            //  modelBuilder.Entity<Person>().HasIndex(person => person.TIN).IsUnique();
            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");

            // Table Relations
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasOne(c => c.Country)
                .WithMany(p => p.Persons)
                .HasForeignKey(p => p.CountryId);
            });
        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }
        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", person.Id),
                new SqlParameter("@Name", person.Name),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@BirthDate", person.BirthDate),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryId", person.CountryId),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson]@Id, @Name, @Email, @BirthDate, @Gender, @CountryId, @Address, @ReceiveNewsLetters", parameters);
        }
    }
}
