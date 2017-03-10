using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrashTalkApi.Models;

namespace TrashTalkApi.Calculations
{
    public class FillCalculator
    {
        private static decimal MAX_TRASH_CAN_HEIGHT = 90;

        public static decimal FillGrade(TrashCanStatus status)
        {
            decimal height = Math.Min(status.Distance.Sensor1, status.Distance.Sensor2);
            if (height <= 0)
                return (decimal) 1.0;
            if (height >= MAX_TRASH_CAN_HEIGHT)
                return 0;
            return MAX_TRASH_CAN_HEIGHT/ height;
        }
    }
}