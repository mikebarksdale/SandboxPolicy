using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;

namespace SandboxPolicy.Services.CommandQuery.Command
{
    public class SavePolicyCommand : BaseQueryCommand
    {
        public Policy Policy { get; set; }
        public Policy OriginalPolicy { get; set; }

        public SavePolicyCommand(SandboxPolicyEntities context) : base(context) { }

        public Policy Execute()
        {
            if (Policy == null)
                throw new Exception("Policy must be set");

            if (OriginalPolicy == null)
                return null;

            //update the root
            _context.Entry(OriginalPolicy).CurrentValues.SetValues(Policy);

            //remove any references
            foreach (var insured in OriginalPolicy.Insured)
                if (!Policy.Insured.Any(i => i.InsuredId == insured.InsuredId))
                    _context.Insured.Remove(insured);

            foreach (var coverage in OriginalPolicy.PolicyCoverage)
                if (!Policy.PolicyCoverage.Any(c => c.PolicyCoverageId == coverage.PolicyCoverageId))
                    _context.PolicyCoverage.Remove(coverage);

            //add/update references
            foreach (var insured in Policy.Insured)
            {
                var originalInsured = _context.Insured.SingleOrDefault(i => i.InsuredId == insured.InsuredId);
                if (originalInsured != null)
                    _context.Entry(originalInsured).CurrentValues.SetValues(insured);
                else //add
                {
                    insured.Policy = OriginalPolicy;
                    insured.Transaction = OriginalPolicy.Transaction;
                    _context.Insured.Add(insured);
                }
            }

            //add update/references
            foreach (var coverage in Policy.PolicyCoverage)
            {
                var originalCoverage = _context.PolicyCoverage.SingleOrDefault(c => c.PolicyCoverageId == coverage.PolicyCoverageId);
                if (originalCoverage != null)
                    _context.Entry(originalCoverage).CurrentValues.SetValues(coverage);
                else //add
                {
                    coverage.Policy = OriginalPolicy;
                    coverage.Transaction = OriginalPolicy.Transaction;
                    _context.PolicyCoverage.Add(coverage);
                }
            }

            _context.SaveChanges();

            return Policy;
        }
    }
}
