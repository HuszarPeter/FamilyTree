using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal.Model;
using Event = FamilyTree.ViewModel.Model.Event;
using GeneratedEvent = FamilyTree.ViewModel.Model.GeneratedEvent;

namespace FamilyTree.ViewModel.Extensions
{
    public static class EventExtensions
    {
        public static Event ConvertToModelEvent(this Dal.Model.Event e)
        {
            return new Event
            {
                Id = e.Id,
                Date = e.Date,
                Description = e.Description
            };
        }

        public static GeneratedEvent ConvertToModelGeneratedEvent(this Dal.Model.GeneratedEvent e)
        {
            return new GeneratedEvent
            {
                Id = e.Id,
                Date = e.Date,
                Description = e.Description,
                PersonId = e.PersonId,
                EventType = (GeneratedEventType)e.GeneratedEventType
            };
        }
    }
}
