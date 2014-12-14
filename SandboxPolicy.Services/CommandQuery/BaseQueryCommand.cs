using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;

namespace SandboxPolicy.Services.CommandQuery
{
    public abstract class BaseQueryCommand
    {
        protected readonly SandboxPolicyEntities _context;

        protected BaseQueryCommand(SandboxPolicyEntities context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }
    }
}
