//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SandboxPolicy.Entities
{
    using System;
    using System.Collections.Generic;
    
    [Serializable]
    public partial class Transaction
    {
        public Transaction()
        {
            this.Insured = new HashSet<Insured>();
            this.Policy = new HashSet<Policy>();
            this.PolicyCoverage = new HashSet<PolicyCoverage>();
        }
    
        public int TransactionId { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string ModifiedUser { get; set; }
        public Nullable<int> TransactionRefId { get; set; }
    
        public virtual ICollection<Insured> Insured { get; set; }
        public virtual ICollection<Policy> Policy { get; set; }
        public virtual ICollection<PolicyCoverage> PolicyCoverage { get; set; }
    }
}
