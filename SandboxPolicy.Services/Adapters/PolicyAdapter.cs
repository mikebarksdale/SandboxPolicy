﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using SandboxPolicy.Models;

namespace SandboxPolicy.Services.Adapters
{
    public static class PolicyAdapter
    {
        public static IPolicyModel ToPolicyModel(Policy policy)
        {
            var policyModel = new PolicyModel
            {
                PolicyId = policy.PolicyId,
                PolicyNumber = policy.PolicyNumber,
                Description = policy.Description,
                Mod = policy.Mod,
                EffectiveDate = policy.EffectiveDate,
                ExpirationDate = policy.ExpirationDate,
                TransactionId = policy.TransactionId,
                Status = policy.Status
            };

            foreach (var insured in policy.Insured)
                policyModel.Insureds.Add(InsuredAdapter.ToInsuredModel(insured));

            foreach (var coverage in policy.PolicyCoverage)
                policyModel.Coverages.Add(CoverageAdapter.ToCoverageModel(coverage));

            return policyModel;
        }

        public static Policy ToPolicy(IPolicyModel policyModel)
        {
            var policy = new Policy();

            policy.PolicyId = policyModel.PolicyId;
            policy.PolicyNumber = policyModel.PolicyNumber;
            policy.Description = policyModel.Description;
            policy.Mod = policyModel.Mod;
            policy.EffectiveDate = policyModel.EffectiveDate;
            policy.ExpirationDate = policyModel.ExpirationDate;
            policy.Status = policyModel.Status;
            policy.TransactionId = policyModel.TransactionId;

            foreach (var insuredModel in policyModel.Insureds)
            {
                var insured = InsuredAdapter.ToInsured(insuredModel);
                insured.PolicyId = policy.PolicyId;
                insured.TransactionId = policy.TransactionId;

                policy.Insured.Add(insured);
            }

            foreach (var coverageModel in policyModel.Coverages)
            {
                var coverage = CoverageAdapter.ToPolicyCoverage(coverageModel);
                coverage.PolicyId = policy.PolicyId;
                coverage.TransactionId = policy.TransactionId;
                policy.PolicyCoverage.Add(coverage);
            }

            return policy;
        }
    }
}
