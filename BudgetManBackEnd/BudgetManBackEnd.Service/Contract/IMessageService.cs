using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IMessageService
    {
        Task<string> HandleMessage(string message, List<byte[]>? images = null, string? userId = null, bool isGroup = false);

        string GetBalance(string userid);
    }
}
