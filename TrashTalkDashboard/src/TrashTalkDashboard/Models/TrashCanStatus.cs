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
        public decimal Weight { get; set; }

        public bool ShouldBeEmptied
        {
            get
            {
                if (FillGrade.HasValue && FillGrade.Value > 80)
                    return true;
                if (Weight > 20)
                    return true;
                return false;
            }
        }

    }
}
