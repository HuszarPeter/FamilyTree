using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;

namespace FamilyTree.ViewModel
{
    public class GenderStatisticsViewModel : ModelBase
    {
        private List<GenderStatistic> _statistics;
        private List<AgeStatistic> _ageStatistics;

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

        public void DownloadData()
        {
            using (var context = new DataContext())
            {
                Statistics = context.GetGenderStatistics();
                AgeStatistics = context.GetAgeStatistics();
            }
        }
    }
}
