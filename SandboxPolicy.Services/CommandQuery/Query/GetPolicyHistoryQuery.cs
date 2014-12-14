using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using SandboxPolicy.Services.Extensions;

namespace SandboxPolicy.Services.CommandQuery.Query
{
    public class GetPolicyHistoryQuery : BaseQueryCommand
    {
        public int TransactionId { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public GetPolicyHistoryQuery(SandboxPolicyEntities context) : base(context) { }

        public IEnumerable<Policy> Execute(bool doPaging)
        {
            var query = _context.Transaction.Where(t => t.TransactionId == TransactionId)
                .OrderByDescending(t => t.TransactionDate)
                .SelectMany(t => t.Policy)
                .OrderByDescending(p =>p.EffectiveDate).ThenByDescending(p => p.Mod);

            return doPaging ?
                query.Page(CurrentPage, PageSize > 0 ? PageSize : 25) :
                query;
        }
    }
}
