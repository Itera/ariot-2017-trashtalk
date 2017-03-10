using System;

namespace TrashTalkApi.Models
{
    public class TrashCanStatus
    {
        public DateTime Timestamp { get; set; }
        public Accelerometer Accelerometer { get; set; }
        public Distance Distance { get; set; }
        public int Flame { get; set; }
        public Temperature Temperature { get; set; }
    }
}
