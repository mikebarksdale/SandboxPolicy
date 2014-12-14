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
    public class GetQuotesForTermQuery : BaseQueryCommand
    {
        public string PolicyNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public GetQuotesForTermQuery(SandboxPolicyEntities context) : base(context) {  }

        public IEnumerable<Policy> Execute(bool doPaging)
        {
            var query = _context.Policy
                .Include(i => i.Insured)
                .Include(c => c.PolicyCoverage)
                .Where(p => p.EffectiveDate == EffectiveDate && p.Status == "Quote" && p.PolicyNumber.Equals(PolicyNumber, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(p => p.Mod);

            return doPaging ? 
                query.Page(CurrentPage, PageSize > 0 ? PageSize : 25) : 
                query;
        }
    }
}
