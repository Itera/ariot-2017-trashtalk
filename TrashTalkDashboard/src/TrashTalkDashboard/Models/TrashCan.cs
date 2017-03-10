using System;
using System.Collections.Generic;

namespace TrashTalkDashboard.Models
{
    public class TrashCan
    {
        public string id { get; set; }
        public TrashCanStatus LatestReading { get; set; }
        public List<TrashCanStatus> TrashCanStatuses { get; set; }
    }
}