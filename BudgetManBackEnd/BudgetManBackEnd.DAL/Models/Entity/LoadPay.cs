﻿using BudgetManBackEnd.CommonClass.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class LoanPay:BaseAccountEntity
    {
        [ForeignKey("Loan")]
        public Guid LoanId { get; set; }
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }
        public double? PaidAmount { get; set; }
        public double? Interest { get; set; }
        public double InterestRate { get; set; } = 0;
        public TimePeriod RatePeriod { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}
