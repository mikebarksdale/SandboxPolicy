using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxPolicy.Models
{
    public interface IInsuredModel
    {
        int InsuredId { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string LastName { get; set; }
        string DbaName { get; set; }
        string InsuredType { get; set; }
        int TransactionId { get; set; }
    }

    public class InsuredModel : IInsuredModel
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
