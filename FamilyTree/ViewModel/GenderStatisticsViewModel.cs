using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;
using FamilyTree.ViewModel.Extensions;
using PersonWithCount = FamilyTree.ViewModel.Model.PersonWithCount;

namespace FamilyTree.ViewModel
{
    public class GenderStatisticsViewModel : ModelBase
    {
        private List<GenderStatistic> _statistics;
        private List<AgeStatistic> _ageStatistics;
        private List<YearStatistics> _eventsByYear;
        private List<PersonWithCount> _mostParticipatingPersons;

        public List<GenderStatistic> Statistics
        {
            get { return _statistics; }
            set
            {
                _statistics = value;
                OnPropertyChanged();
            }
        }

        public List<AgeStatistic> AgeStatistics
        {
            get { return _ageStatistics; }
            set
            {
                _ageStatistics = value;
                OnPropertyChanged();
            }
        }

        public List<YearStatistics> EventsByYear
        {
            get { return _eventsByYear; }
            set
            {
                _eventsByYear = value;
                OnPropertyChanged();
            }
        }

        public List<PersonWithCount> MostParticipatingPersons
        {
            get { return _mostParticipatingPersons; }
            set
            {
                _mostParticipatingPersons = value;
                OnPropertyChanged();
            }
        }

        public void DownloadData()
        {
            using (var context = new DataContext())
            {
                Statistics = context.GetGenderStatistics();
                AgeStatistics = context.GetAgeStatistics();
                EventsByYear = context.GetEventStatsByYear();
                MostParticipatingPersons = context.GetMostParticipation()
                    .Select(p => p.ConvertToViewPersonWithCount()).ToList();
            }
        }
    }
}
