using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Common.Enum
{
    public enum ChangeType
    {
        Spent=-1, TransferOut=-2, Loan=-3, LoandPaid=4,
        Income=1, TransferIn=2,   Debt=3, DebtPaid=-4,
    }
}
