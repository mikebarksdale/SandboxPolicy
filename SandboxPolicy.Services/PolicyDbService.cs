using System;
using System.Linq;
using SandboxPolicy.Entities;
using SandboxPolicy.Models;
using SandboxPolicy.Services.Adapters;
using SandboxPolicy.Services.Extensions;

namespace SandboxPolicy.Services
{
    public class PolicyDbService : IPolicyService
    {
        private readonly SandboxPolicyEntities _context;

        public PolicyDbService(SandboxPolicyEntities context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public IPolicyModel CreatePolicy(IPolicyModel policyModel)
        {
            if (policyModel == null)
                throw new ArgumentNullException("policyModel");

            //create a transaction record
            var transaction = new Transaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "WIP",
                ModifiedUser = !string.IsNullOrEmpty(policyModel.User) ? policyModel.User : "system"
            };

            //generate a temporary policy number
            var policy = PolicyAdapter.ToPolicy(policyModel);
            policy.PolicyNumber = "T" + new Random().Next(100000, 999999); //this is terrible, but whatever
            policy.Mod = 0;
            policy.Status = "Quote";

            transaction.Policy.Add(policy);
            _context.Transaction.Add(transaction);
            _context.SaveChanges();

            return PolicyAdapter.ToPolicyModel(policy);
        }

        public IPolicyModel UpdatePolicy(IPolicyModel policyModel)
        {
            if (policyModel == null)
                throw new ArgumentNullException("policyModel");

            var query =
                _context.Policy.Where(
                    p =>
                    p.PolicyId == policyModel.PolicyId &&
                    p.Status.Equals(policyModel.Status, StringComparison.InvariantCultureIgnoreCase));

            var originalPolicy = policyModel.Mod > 0 ? query.Where(p => p.Mod == policyModel.Mod)
                .OrderByDescending(p => p.Mod)
                .FirstOrDefault()
                    : query.FirstOrDefault();

            if (originalPolicy == null)
                return null;

            var policy = PolicyAdapter.ToPolicy(policyModel);

            //update the root
            _context.Entry(originalPolicy).CurrentValues.SetValues(policy);

            //remove any references
            foreach (var insured in originalPolicy.Insured.Where(insured => policy.Insured.All(i => i.InsuredId != insured.InsuredId)))
                _context.Insured.Remove(insured);

            foreach (var coverage in originalPolicy.PolicyCoverage.Where(coverage => policy.PolicyCoverage.All(c => c.PolicyCoverageId != coverage.PolicyCoverageId)))
                _context.PolicyCoverage.Remove(coverage);

            //add/update references
            foreach (var insured in policy.Insured)
            {
                var originalInsured = _context.Insured.SingleOrDefault(i => i.InsuredId == insured.InsuredId);
                if (originalInsured != null)
                    _context.Entry(originalInsured).CurrentValues.SetValues(insured);
                else //add
                {
                    insured.Policy = originalPolicy;
                    insured.Transaction = originalPolicy.Transaction;
                    _context.Insured.Add(insured);
                }
            }

            //add update/references
            foreach (var coverage in policy.PolicyCoverage)
            {
                var originalCoverage = _context.PolicyCoverage.SingleOrDefault(c => c.PolicyCoverageId == coverage.PolicyCoverageId);
                if (originalCoverage != null)
                    _context.Entry(originalCoverage).CurrentValues.SetValues(coverage);
                else //add
                {
                    coverage.Policy = originalPolicy;
                    coverage.Transaction = originalPolicy.Transaction;
                    _context.PolicyCoverage.Add(coverage);
                }
            }

            _context.SaveChanges();

            return PolicyAdapter.ToPolicyModel(originalPolicy);
        }

        public IPolicyModel CopyQuote(int policyId, int mod)
        {
            var query = _context.Policy.Where(
                p => p.PolicyId == policyId && p.Status.Equals("Quote", StringComparison.InvariantCultureIgnoreCase));

            var quote = mod > 0
                            ? query.Where(p => p.Mod == mod).OrderByDescending(p => p.Mod).FirstOrDefault()
                            : query.FirstOrDefault();

            if (quote == null)
                return null;

            //copy the properties using serializable classes.. quick and dirty
            var copiedQuote = quote.Clone();
            copiedQuote.Mod = _context.Policy.Where(
                p => p.PolicyNumber.Equals(quote.PolicyNumber, StringComparison.InvariantCultureIgnoreCase) &&
                     p.Status.Equals("Quote", StringComparison.InvariantCultureIgnoreCase))
                .Max(p => p.Mod) + 1;

            //disassociate from cloned object's transaction
            copiedQuote.Transaction = null;
            copiedQuote.Insured.ToList().ForEach(i => i.Transaction = null);
            copiedQuote.PolicyCoverage.ToList().ForEach(c => c.Transaction = null);

            _context.Policy.Add(copiedQuote);
            _context.SaveChanges();

            return PolicyAdapter.ToPolicyModel(copiedQuote);
        }

        public IPolicyModel IssueQuote(IPolicyModel policyModel)
        {
            if (policyModel == null)
                throw new ArgumentNullException("policyModel");

            var query =
                _context.Policy.Where(
                    p =>
                    p.PolicyId == policyModel.PolicyId &&
                    p.Status.Equals(policyModel.Status, StringComparison.InvariantCultureIgnoreCase));

            var originalPolicy = policyModel.Mod > 0 ? query.Where(p => p.Mod == policyModel.Mod)
                .OrderByDescending(p => p.Mod)
                .First()
                    : query.First();

            //if the current transaction is a renewal, get the latest mod number and increment by 1, else, set to 1
            var newMod = 1;
            if (originalPolicy.Transaction.TransactionType.Equals("WIPRenewal", StringComparison.InvariantCultureIgnoreCase))
            {
                var latestIssuedPolicy =
                    _context.Policy.Where(p => p.TransactionId == originalPolicy.Transaction.TransactionRefId &&
                                                             p.Status.Equals("Issued", StringComparison.InvariantCultureIgnoreCase))
                                                .OrderByDescending(p => p.Mod)
                                                .FirstOrDefault();
                if (latestIssuedPolicy != null)
                    newMod = latestIssuedPolicy.Mod++;
            }

            //create a transaction record
            var transaction = new Transaction
            {
                TransactionRefId = originalPolicy.TransactionId,
                TransactionDate = DateTime.Now,
                TransactionType = "Issued",
                ModifiedUser = !string.IsNullOrEmpty(policyModel.User) ? policyModel.User : "system"
            };

            //copy the records and associate the new transaction record
            var issuedPolicy = originalPolicy.Clone();
            issuedPolicy.PolicyNumber = issuedPolicy.PolicyNumber.Replace("T", "P");
            issuedPolicy.Transaction = transaction;
            issuedPolicy.Insured.ToList().ForEach(i => i.Transaction = transaction);
            issuedPolicy.PolicyCoverage.ToList().ForEach(c => c.Transaction = transaction);

            issuedPolicy.Status = "Issued";
            issuedPolicy.Mod = newMod;
            transaction.Policy.Add(issuedPolicy);

            _context.Transaction.Add(transaction);
            _context.SaveChanges();

            return PolicyAdapter.ToPolicyModel(issuedPolicy);
        }

        public void RenewPolicy()
        {
            throw new NotImplementedException();
        }

        public void GetPolicies()
        {
            throw new NotImplementedException();
        }

        public void GetQuotesForPolicy()
        {
            throw new NotImplementedException();
        }
    }
}
