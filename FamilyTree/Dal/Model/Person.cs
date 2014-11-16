using System;
using System.Collections.Generic;
using FamilyTree.Utils;

namespace FamilyTree.Dal.Model
{
    [DatabaseTable(Name = "szemely")]
    public class Person
    {
        [DatabaseField(Name = "szemely_id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [DatabaseField(Name = "keresztnev")]
        public string FirstName { get; set; }

        [DatabaseField(Name = "vezeteknev")]
        public string LastName { get; set; }

        [DatabaseField(Name = "szuletes_ideje")]
        public DateTime? DateOfBirth { get; set; }

        [DatabaseField(Name = "halalozas_ideje")]
        public DateTime? DateOfDeath { get; set; }

        [DatabaseField(Name = "ferfi")]
        public bool IsMale { get; set; }

        [DatabaseField(Name="portre")]
        public byte[] ProfilePicture { get; set; }

        [DatabaseField(Name = "leanykori_keresztnev")]
        public string BirthFirstName { get; set; }

        [DatabaseField(Name = "leanykori_vezeteknev")]
        public string BirthLastName { get; set; }
    }
}
