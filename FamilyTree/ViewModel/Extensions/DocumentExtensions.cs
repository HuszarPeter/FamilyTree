using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.ViewModel.Extensions
{
    public static class DocumentExtensions
    {
        public static EventDocument ConvertToviewModelDocument(this Dal.Model.EventDocument doc)
        {
            return new EventDocument
            {
                Id = doc.Id,
                Data = doc.Data,
                EventId = doc.EventId,
                FileName = doc.FileName,
                FileType = doc.FileType
            };
        }

        public static Dal.Model.EventDocument ConvertBackToDalModel(this EventDocument doc)
        {
            return new Dal.Model.EventDocument
            {
                Id = doc.Id,
                Data = doc.Data,
                EventId = doc.EventId,
                FileName = doc.FileName,
                FileType = doc.FileType
            };
        }
    }
}
