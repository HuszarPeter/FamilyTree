using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Dal.Model
{
    [DatabaseTable(Name = "dokumentum")]
    public class EventDocument
    {
        [DatabaseField(Name = "dokumentum_id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [DatabaseField(Name = "esemeny_id")]
        public int EventId { get; set; }

        [DatabaseField(Name = "tipus")]
        public string FileType { get; set; }

        [DatabaseField(Name = "filenev")]
        public string FileName { get; set; }

        [DatabaseField(Name = "adat")]
        public byte[] Data { get; set; }
    }

    public class EventDocumentComparer : IEqualityComparer<EventDocument>
    {
        public bool Equals(EventDocument x, EventDocument y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(EventDocument obj)
        {
            return obj.Id;
        }
    }
}
