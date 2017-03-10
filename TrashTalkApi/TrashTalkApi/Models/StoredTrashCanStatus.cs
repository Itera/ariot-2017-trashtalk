using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrashTalkApi.Models
{
    public class StoredTrashCanStatus : TrashCanStatus
    {
        public Decimal? FillGrade {get; set;}
    }
}