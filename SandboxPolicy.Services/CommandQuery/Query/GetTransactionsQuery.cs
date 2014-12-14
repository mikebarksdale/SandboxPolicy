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
    public class GetTransactionsQuery : BaseQueryCommand
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public GetTransactionsQuery(SandboxPolicyEntities context) : base(context) {  }

        public IEnumerable<Transaction> Execute(bool doPaging)
        {
            var query = _context.Transaction
                .Include(p => p.Policy)
                .Include(p => p.Policy.Select(i => i.Insured))
                .Include(p => p.Policy.Select(c => c.PolicyCoverage))
                .OrderByDescending(t => t.TransactionDate);

            return doPaging ?
                query.Page(CurrentPage, PageSize > 0 ? PageSize : 25) :
                query;
        }
    }
}
