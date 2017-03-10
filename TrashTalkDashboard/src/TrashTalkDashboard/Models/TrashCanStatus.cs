using System;

namespace TrashTalkDashboard.Models
{
    public class TrashCanStatus
    {
        public decimal? FillGrade { get; set; }
        public DateTime Timestamp { get; set; }
        public Accelerometer Accelerometer { get; set; }
        public Distance Distance { get; set; }
        public int Flame { get; set; }
        public Temperature Temperature { get; set; }
        public bool LidIsClosed { get; set; }
    }
}
