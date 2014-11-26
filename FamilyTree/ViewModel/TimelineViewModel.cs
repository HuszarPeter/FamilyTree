using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal;
using FamilyTree.Utils;
using FamilyTree.ViewModel.Extensions;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.ViewModel
{
    public class TimelineViewModel : ModelBase
    {
        private List<GeneratedEvent> _timelineEvents;

        public List<GeneratedEvent> TimelineEvents
        {
            get { return _timelineEvents; }
            set
            {
                _timelineEvents = value;
                OnPropertyChanged();
            }
        }

        public void DownloadGeneratedEvents()
        {
            using (var context = new DataContext())
            {
                TimelineEvents = context.GetCombinedEventList()
                    .Select(e => e.ConvertToModelGeneratedEvent())
                    .ToList();
            }
        }
    }
}
