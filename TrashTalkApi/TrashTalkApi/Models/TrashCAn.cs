using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrashTalkApi.Models
{
    public class TrashCan
    {
        public string id { get; set; }
        public TrashCanStatus LatestReading { get; set; }
        public List<TrashCanStatus> TrashCanStatuses { get; set; }
    }
}