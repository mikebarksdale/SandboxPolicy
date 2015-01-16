using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxPolicy.Models
{
    public interface IPolicyModel : IAuditModel
    {
        IList<IInsuredModel> Insureds { get; set; }
        IList<ICoverageModel> Coverages { get; set; }
        int PolicyId { get; set; }
        string PolicyNumber { get; set; }
        int Mod { get; set; }
        string Status { get; set; }
        string Description { get; set; }
        DateTime EffectiveDate { get; set; }
        DateTime? ExpirationDate { get; set; }
        int TransactionId { get; set; }
    }

    public class PolicyModel : IPolicyModel
    {
        public IList<IInsuredModel> Insureds { get; set; }
        public IList<ICoverageModel> Coverages { get; set; }

        public PolicyModel()
        {
            Insureds = new List<IInsuredModel>();
            Coverages = new List<ICoverageModel>();
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
