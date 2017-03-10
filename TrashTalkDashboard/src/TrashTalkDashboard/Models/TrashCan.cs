using System;
using System.Collections.Generic;

namespace TrashTalkDashboard.Models
{
    public class TrashCan
    {
        public TrashCan()
        {
            LatestReading = new TrashCanStatus();
            TrashCanStatuses = new List<TrashCanStatus>();
        }
        public string id { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public TrashCanStatus LatestReading { get; set; }
        public List<TrashCanStatus> TrashCanStatuses { get; set; }
    }
}