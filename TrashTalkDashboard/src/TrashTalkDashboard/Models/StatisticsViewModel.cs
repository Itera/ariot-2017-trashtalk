using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrashTalkDashboard.Models
{
    public class StatisticsViewModel
    {
        public decimal AverageFillPercentage { get; set; }
        public string CircleClassFillPercentage { get; set; }
        public decimal AverageWeight { get; set; }
        public string CircleClassWeigth { get; set; }

        public List<TrashCan> TrashCans { get; set; }
    }
}
