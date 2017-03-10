using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrashTalkApi.Models;

namespace TrashTalkApi.Calculations
{
    public class TrashCanReadingTransformer
    {
        public static StoredTrashCanStatus CalculateStoreModel(TrashCanStatus incomingStatus)
        {
            Decimal fillGrade = FillCalculator.FillGrade(incomingStatus);

            StoredTrashCanStatus result = new StoredTrashCanStatus();

            result.Accelerometer = incomingStatus.Accelerometer;
            result.Distance = incomingStatus.Distance;
            result.Temperature = incomingStatus.Temperature;
            result.Timestamp = incomingStatus.Timestamp;
            result.Flame = incomingStatus.Flame;
            result.LidIsClosed = incomingStatus.LidIsClosed;
            result.FillGrade = fillGrade;

            return result;
        }
    }
}