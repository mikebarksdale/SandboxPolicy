using System;
using SandboxPolicy.Entities;
using SandboxPolicy.Models;
using SandboxPolicy.Services.Adapters;
using SandboxPolicy.Services.CommandQuery.Command;
using SandboxPolicy.Services.CommandQuery.Query;

namespace SandboxPolicy.Services.Service
{
    public class PolicyService : IPolicyService
    {
        private readonly SandboxPolicyEntities _context;

        public PolicyService(SandboxPolicyEntities context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public IPolicyModel CreatePolicy(IPolicyModel policyModel)
        {
            if (policyModel == null)
                throw new ArgumentNullException("policyModel");

            var createPolicyCommand = new CreatePolicyCommand(_context)
            {
                Policy = PolicyAdapter.ToPolicy(policyModel),
                TransactionStatus = "WIP",
                User = policyModel.User
            };

            var policy = createPolicyCommand.Execute();

            return PolicyAdapter.ToPolicyModel(policy);
        }

        public IPolicyModel UpdatePolicy(IPolicyModel policyModel)
        {
            if (policyModel == null)
                throw new ArgumentNullException("policyModel");

            var getPolicyQuery = new GetPolicyQuery(_context)
            {
                PolicyId = policyModel.PolicyId,
                Mod = policyModel.Mod,
                PolicyStatus = policyModel.Status
            };

            var originalPolicy = getPolicyQuery.Execute();

            var updatePolicyCommand = new SavePolicyCommand(_context)
            {
                Policy = PolicyAdapter.ToPolicy(policyModel),
                OriginalPolicy = originalPolicy
            };

            var policy = updatePolicyCommand.Execute();

            return PolicyAdapter.ToPolicyModel(policy);
        }

        public IPolicyModel CopyQuote(int policyId, int mod)
        {
            var getPolicyQuery = new GetPolicyQuery(_context)
            {
                PolicyId = policyId,
                Mod = mod > 0 ? mod : -1,
                PolicyStatus = "Quote"
            };

            var quote = getPolicyQuery.Execute();

            if (quote == null)
                return null;

            var copyQuoteCommand = new CopyQuoteCommand(_context)
            {
                Quote = quote
            };

            var copiedQuote = copyQuoteCommand.Execute();

            return PolicyAdapter.ToPolicyModel(copiedQuote);
        }

        public IPolicyModel IssueQuote(IPolicyModel policyModel)
        {
            if (policyModel == null)
                throw new ArgumentNullException("policyModel");

            var getPolicyQuery = new GetPolicyQuery(_context)
            {
                PolicyId = policyModel.PolicyId,
                Mod = policyModel.Mod,
                PolicyStatus = "Quote"
            };

            var originalPolicy = getPolicyQuery.Execute();

            var issueQuoteCommand = new IssuePolicyCommand(_context)
            {
                Policy = originalPolicy,
                User = policyModel.User
            };

            var policy = issueQuoteCommand.Execute();

            return PolicyAdapter.ToPolicyModel(policy);
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
