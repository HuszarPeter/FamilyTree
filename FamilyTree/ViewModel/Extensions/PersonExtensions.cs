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

        public static Dal.Model.Person ConvertToDalPerson(this Person p)
        {
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
            };
        }
    }
}
