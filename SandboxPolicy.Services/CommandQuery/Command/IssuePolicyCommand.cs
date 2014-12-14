using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using SandboxPolicy.Services.Extensions;

namespace SandboxPolicy.Services.CommandQuery.Command
{
    public class IssuePolicyCommand : BaseQueryCommand
    {
        public Policy Policy { get; set; }
        public string User { get; set; }

        public IssuePolicyCommand(SandboxPolicyEntities context) : base(context) { }

        public Policy Execute()
        {
            //if the current transaction is a renewal, get the latest mod number and increment by 1, else, set to 1
            var newMod = 1;
            if (Policy.Transaction.TransactionType.Equals("WIPRenewal", StringComparison.InvariantCultureIgnoreCase))
            {
                var latestIssuedPolicy = _context.Policy.Where(p => p.TransactionId == Policy.Transaction.TransactionRefId && 
                                                                    p.Status.Equals("Issued", StringComparison.InvariantCultureIgnoreCase))
                                                        .OrderByDescending(p => p.Mod)
                                                        .FirstOrDefault();
                if (latestIssuedPolicy != null)
                    newMod = latestIssuedPolicy.Mod++;
            }

            //create a transaction record
            var transaction = new Transaction();
            transaction.TransactionRefId = Policy.TransactionId;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionType = "Issued";
            transaction.ModifiedUser = !string.IsNullOrEmpty(User) ? User : "system";
            
            //copy the records and associate the new transaction record
            var issuedPolicy = Policy.Clone();
            issuedPolicy.PolicyNumber = issuedPolicy.PolicyNumber.Replace("T", "P");
            issuedPolicy.Transaction = transaction;
            issuedPolicy.Insured.ToList().ForEach(i => i.Transaction = transaction);
            issuedPolicy.PolicyCoverage.ToList().ForEach(c => c.Transaction = transaction);

            issuedPolicy.Status = "Issued";
            issuedPolicy.Mod = newMod;
            transaction.Policy.Add(issuedPolicy);

            _context.Transaction.Add(transaction);
            _context.SaveChanges();

            return issuedPolicy;
        }
    }
}
