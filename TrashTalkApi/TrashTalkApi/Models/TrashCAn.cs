using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrashTalkApi.Models
{
    public class TrashCan
    {
        public string id { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public string Address { get; set; }
        public StoredTrashCanStatus LatestReading { get; set; }
        public List<StoredTrashCanStatus> TrashCanStatuses { get; set; }
    }
}