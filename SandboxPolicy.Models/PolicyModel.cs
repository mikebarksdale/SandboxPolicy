using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxPolicy.Models
{
    public class PolicyModel : IAuditModel
    {
        public IList<InsuredModel> Insureds;
        public IList<CoverageModel> Coverages;

        public PolicyModel()
        {
            Insureds = new List<InsuredModel>();
            Coverages = new List<CoverageModel>();
        }

        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
        public int Mod { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int TransactionId { get; set; }

        public string User { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
