using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using SandboxPolicy.Services.Extensions;
using System.Data.Entity;

namespace SandboxPolicy.Services.CommandQuery.Query
{
    public class GetPolicyQuery : BaseQueryCommand
    {
        public int PolicyId { get; set; }
        public int Mod { get; set; }
        public string PolicyStatus { get; set; }

        public GetPolicyQuery(SandboxPolicyEntities context) : base(context) { }

        public Policy Execute()
        {
            var query = _context.Policy
                .Include(t => t.Transaction)
                .Include(i => i.Insured)
                .Include(c => c.PolicyCoverage)
                .Where(p => p.PolicyId == PolicyId && p.Status.Equals(PolicyStatus, StringComparison.InvariantCultureIgnoreCase));

            return Mod > 0 ? query.Where(p => p.Mod == Mod)
                .OrderByDescending(p => p.Mod)
                .FirstOrDefault() 
                    : query.FirstOrDefault();
        }
    }
}
