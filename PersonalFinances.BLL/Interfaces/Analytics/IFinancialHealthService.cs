using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinances.BLL.Entities.Models.Analytics;

namespace PersonalFinances.BLL.Interfaces.Analytics
{
    public interface IFinancialHealthService
    {
        Task<FinancialHealthModel> CalculateFinancialHealthAsync(string userId);
        Task<FinancialHealthModel> GetLatestFinancialHealthAsync(string userId);
        Task<IEnumerable<FinancialHealthModel>> GetFinancialHealthHistoryAsync(string userId, int months);
        Task SaveFinancialHealthAsync(FinancialHealthModel healthModel);
    }

}
