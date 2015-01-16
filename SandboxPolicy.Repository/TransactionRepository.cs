using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;

namespace SandboxPolicy.Repository
{
    public class TransactionRepository : BaseRepository<Transaction>
    {
        public TransactionRepository(DbContext context) : base(context)
        {
        }
    }
}
