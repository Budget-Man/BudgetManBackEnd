﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class BudgetCategory:BaseEntity
    {
        public int Name { get; set; }

        [ForeignKey("Account")]
        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public AccountInfo Account { get; set; }
    }
}
