using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using System.Data.Entity;

namespace SandboxPolicy.Services.CommandQuery.Command
{
    public class CreatePolicyCommand : BaseQueryCommand
    {
        public Policy Policy { get; set; }
        public string TransactionStatus { get; set; }
        public string User { get; set; }

        public CreatePolicyCommand(SandboxPolicyEntities context) : base(context) {  }

        public Policy Execute()
        {
            if (Policy == null)
                throw new ArgumentNullException("policy");

            //more validations, of course
            if (Policy.EffectiveDate == DateTime.MinValue)
                throw new ArgumentException("Policy effective date must be set.");

            //create a transaction record
            var transaction = new Transaction();
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionType = !string.IsNullOrEmpty(TransactionStatus) ? TransactionStatus : "WIP";
            transaction.ModifiedUser = !string.IsNullOrEmpty(User) ? User : "system";

            //generate a temporary policy number
            Policy.PolicyNumber = "T" + new Random().Next(100000, 999999); //this is terrible, but whatever
            Policy.Mod = 0;
            Policy.Status = "Quote";

            transaction.Policy.Add(Policy);
            _context.Transaction.Add(transaction);
            _context.SaveChanges();

            return Policy;
        }
    }
}
