using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxPolicy.Models
{
    public class CoverageModel
    {
        public int CoverageId { get; set; }
        public string Coverage { get; set; }
        public decimal Limit { get; set; }
        public decimal Deductible { get; set; }
        public int TransactionId { get; set; }
    }
}
