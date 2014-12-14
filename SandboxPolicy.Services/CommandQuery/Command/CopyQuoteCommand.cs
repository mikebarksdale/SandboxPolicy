using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using SandboxPolicy.Services.Extensions;

namespace SandboxPolicy.Services.CommandQuery.Command
{
    public class CopyQuoteCommand : BaseQueryCommand
    {
        public Policy Quote { get; set; }

        public CopyQuoteCommand(SandboxPolicyEntities context) : base(context) { }

        public Policy Execute()
        {
            //copy the properties using serializable classes.. quick and dirty
            var copiedQuote = Quote.Clone();
            copiedQuote.Mod = _context.Policy
                .Where(p => p.PolicyNumber.Equals(Quote.PolicyNumber, StringComparison.InvariantCultureIgnoreCase) && 
                            p.Status.Equals("Quote", StringComparison.InvariantCultureIgnoreCase))
                .Max(p => p.Mod) + 1;

            //disassociate from cloned object's transaction
            copiedQuote.Transaction = null;
            copiedQuote.Insured.ToList().ForEach(i => i.Transaction = null);
            copiedQuote.PolicyCoverage.ToList().ForEach(c => c.Transaction = null);

            _context.Policy.Add(copiedQuote);
            _context.SaveChanges();

            return copiedQuote;
        }
    }
}
