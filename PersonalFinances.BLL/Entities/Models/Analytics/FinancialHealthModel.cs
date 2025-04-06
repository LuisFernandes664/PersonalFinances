using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Analytics
{
    public class FinancialHealthModel : BaseEntity
    {
        public string UserId { get; set; }
        public int OverallScore { get; set; }
        public int SavingsScore { get; set; }
        public int SpendingScore { get; set; }
        public int DebtScore { get; set; }
        public int BudgetAdherenceScore { get; set; }
        public List<FinancialHealthRecommendation> Recommendations { get; set; }
        public DateTime CalculatedAt { get; set; }

        public FinancialHealthModel() : base()
        {
            UserId = string.Empty;
            Recommendations = new List<FinancialHealthRecommendation>();
            CalculatedAt = DateTime.UtcNow;
        }

        public FinancialHealthModel(DataRow row) : base(row)
        {
            UserId = row.Field<string>("user_id") ?? string.Empty;
            OverallScore = row.Field<int>("overall_score");
            SavingsScore = row.Field<int>("savings_score");
            SpendingScore = row.Field<int>("spending_score");
            DebtScore = row.Field<int>("debt_score");
            BudgetAdherenceScore = row.Field<int>("budget_adherence_score");
            CalculatedAt = row.Field<DateTime>("calculated_at");
            Recommendations = new List<FinancialHealthRecommendation>();
        }
    }

    public class FinancialHealthRecommendation
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string ActionItem { get; set; }
        public int PriorityLevel { get; set; }

        public FinancialHealthRecommendation()
        {
            Category = string.Empty;
            Description = string.Empty;
            ActionItem = string.Empty;
            PriorityLevel = 2;
        }

        public FinancialHealthRecommendation(DataRow row)
        {
            Category = row.Field<string>("category") ?? string.Empty;
            Description = row.Field<string>("description") ?? string.Empty;
            ActionItem = row.Field<string>("action_item") ?? string.Empty;
            PriorityLevel = row.Field<int>("priority_level");
        }
    }

}
