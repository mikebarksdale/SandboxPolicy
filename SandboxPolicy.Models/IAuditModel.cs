using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxPolicy.Models
{
    public interface IAuditModel
    {
        string User { get; set; }
        DateTime ModifiedDate { get; set; }
    }
}
