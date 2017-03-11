using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrashTalkDashboard.Models.HeatMapViewModels
{
    public class HeatMapPoint
    {
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public decimal FillGrade { get; set; }
    }
}
