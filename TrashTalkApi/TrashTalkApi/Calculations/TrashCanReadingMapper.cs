using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrashTalkApi.Models;

namespace TrashTalkApi.Calculations
{
    public static class TrashCanReadingMapper
    {
        public static StoredTrashCanStatus MapToStoredCanStatus(TrashCanStatus incomingStatus)
        {
            var fillGrade = FillCalculator.FillGrade(incomingStatus);

            var result = new StoredTrashCanStatus
            {
                Accelerometer = incomingStatus.Accelerometer,
                Distance = incomingStatus.Distance,
                Temperature = incomingStatus.Temperature,
                Timestamp = incomingStatus.Timestamp,
                Flame = incomingStatus.Flame,
                LidIsClosed = incomingStatus.LidIsClosed,
                FillGrade = fillGrade
            };


            return result;
        }
    }
}