﻿using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContragentAnalyse.Model.Entities
{
    public class Criteria: NamedEntity
    {
        public float Weight { get; set; }
       
        

    }
}
