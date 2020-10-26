﻿using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class PrescoringScoringHistory : BaseEntity
    {
        public DateTime DatePresScor { get; set; }
        public int Employee_Id { get; set; }
        public int Client_Id { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(Employee_Id))]
        public virtual Employees Employees { get; set; }
    }
}