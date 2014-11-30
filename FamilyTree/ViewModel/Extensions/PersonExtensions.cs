using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.ViewModel.Extensions
{
    public static class PersonExtensions
    {
        public static Person ConvertToViewPerson(this Dal.Model.Person p)
        {
            return new Person
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                BirthFirstName = p.BirthFirstName,
                BirthLastName = p.BirthLastName,
                DateOfBirth = p.DateOfBirth,
                DateOfDeath = p.DateOfDeath,
                Picture = p.ProfilePicture,
                Gender = p.IsMale ? Gender.Male : Gender.Female
            };
        }

        public static PersonWithCount ConvertToViewPersonWithCount(this Dal.Model.PersonWithCount p)
        {
            return new PersonWithCount
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                BirthFirstName = p.BirthFirstName,
                BirthLastName = p.BirthLastName,
                DateOfBirth = p.DateOfBirth,
                DateOfDeath = p.DateOfDeath,
                Picture = p.ProfilePicture,
                Gender = p.IsMale ? Gender.Male : Gender.Female,
                Count = p.Count
            };
        }

        public static Dal.Model.Person ConvertToDalPerson(this Person p)
        {
            if (p == null) return null;
            return new Dal.Model.Person
            {
                Id = p.Id,
                BirthFirstName = p.BirthFirstName,
                BirthLastName = p.BirthLastName,
                DateOfBirth = p.DateOfBirth,
                DateOfDeath = p.DateOfDeath,
                FirstName = p.FirstName,
                LastName = p.LastName,
                IsMale = p.Gender == Gender.Male,
                ProfilePicture = p.Picture
            };
        }

        public static PersonWithCount ConvertToViewPersonWithCount(this Dal.Model.PersonIdAndCounter p)
        {
            var person = LocalDataStorage.Instance.Persons.FirstOrDefault(x => x.Id == p.PersonId);
            if (person == null) return null;

            return new PersonWithCount
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                BirthFirstName = person.BirthFirstName,
                BirthLastName = person.BirthLastName,
                DateOfBirth = person.DateOfBirth,
                DateOfDeath = person.DateOfDeath,
                Picture = person.Picture,
                Gender = person.IsMale ? Gender.Male : Gender.Female,
                Count = p.Counter
            };
        }
    }
}
