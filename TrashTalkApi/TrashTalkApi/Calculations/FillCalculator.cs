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
            if (!status.LidIsClosed)
                return (decimal) -1.0;

            decimal height = (status.Distance.Sensor1 + status.Distance.Sensor2) / 2;
            if (height <= 0 || height >= MAX_TRASH_CAN_HEIGHT)
                return (decimal) -1.0;

            return 1 - (height / MAX_TRASH_CAN_HEIGHT);
        }
    }
}