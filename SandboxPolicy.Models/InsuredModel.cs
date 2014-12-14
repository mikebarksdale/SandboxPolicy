using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxPolicy.Models
{
    public class InsuredModel
    {
        public int InsuredId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DbaName { get; set; }
        public string InsuredType { get; set; }
        public int TransactionId { get; set; }
    }
}
