using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using SandboxPolicy.Models;
using SandboxPolicy.Repository;
using SandboxPolicy.Services.Adapters;
using SandboxPolicy.Services.Extensions;

namespace SandboxPolicy.Services.Service
{
    public class PolicyUnitOfWorkService : IPolicyService
    {
        private readonly UnitOfWork _unitOfWork;

        public PolicyUnitOfWorkService(UnitOfWork unitOfWork)
        {
            if(unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _unitOfWork = unitOfWork;
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
            _unitOfWork.TransactionRepository.Insert(transaction);
            _unitOfWork.Save();

            return PolicyAdapter.ToPolicyModel(policy);
        }

        public IPolicyModel UpdatePolicy(IPolicyModel policyModel)
        {
            if (policyModel == null)
                throw new ArgumentNullException("policyModel");

            var query =
                _unitOfWork.PolicyRepository.Filter(
                    p =>
                    p.PolicyId == policyModel.PolicyId &&
                    p.Status.Equals(policyModel.Status, StringComparison.InvariantCultureIgnoreCase));

            var originalPolicy = policyModel.Mod > 0 ? query.Where(p => p.Mod == policyModel.Mod)
                .OrderByDescending(p => p.Mod)
                .FirstOrDefault()
                    : query.FirstOrDefault();

            //TODO: complete

            _unitOfWork.Save();

            return originalPolicy != null ? PolicyAdapter.ToPolicyModel(originalPolicy) : null;
        }

        public IPolicyModel CopyQuote(int policyId, int mod)
        {
            var query = _unitOfWork.PolicyRepository.Filter(
                p => p.PolicyId == policyId && p.Status.Equals("Quote", StringComparison.InvariantCultureIgnoreCase));

            var quote = mod > 0
                            ? query.Where(p => p.Mod == mod).OrderByDescending(p => p.Mod).FirstOrDefault()
                            : query.FirstOrDefault();

            if (quote == null)
                return null;

            //copy the properties using serializable classes.. quick and dirty
            var copiedQuote = quote.Clone();
            copiedQuote.Mod = _unitOfWork.PolicyRepository.Filter(
                p => p.PolicyNumber.Equals(quote.PolicyNumber, StringComparison.InvariantCultureIgnoreCase) &&
                     p.Status.Equals("Quote", StringComparison.InvariantCultureIgnoreCase))
                .Max(p => p.Mod) + 1;

            //disassociate from cloned object's transaction
            copiedQuote.Transaction = null;
            copiedQuote.Insured.ToList().ForEach(i => i.Transaction = null);
            copiedQuote.PolicyCoverage.ToList().ForEach(c => c.Transaction = null);

            _unitOfWork.PolicyRepository.Insert(copiedQuote);
            _unitOfWork.Save();

            return PolicyAdapter.ToPolicyModel(copiedQuote);
        }

        public IPolicyModel IssueQuote(IPolicyModel policyModel)
        {
            if (policyModel == null)
                throw new ArgumentNullException("policyModel");

            var query =
                _unitOfWork.PolicyRepository.Filter(
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
                    _unitOfWork.PolicyRepository.Filter(p => p.TransactionId == originalPolicy.Transaction.TransactionRefId &&
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

            _unitOfWork.TransactionRepository.Insert(transaction);
            _unitOfWork.Save();

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
