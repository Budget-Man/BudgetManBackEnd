using MayNghien.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Model.Dto
{
    public class LocalTransferDto : BaseDto
    {
     
        public Guid FromMoneyHolderId { get; set; }
        public string? MoneyHolderIdName { get; set; }
           
        public Guid ToMoneyHolderId { get; set; }
       public string? MoneyHolderIdBankName { get; set; }
       
        public double Amount { get; set; }
    }
}
