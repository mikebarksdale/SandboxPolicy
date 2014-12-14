using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using SandboxPolicy.Models;

namespace SandboxPolicy.Services.Adapters
{
    public static class CoverageAdapter
    {
        public static CoverageModel ToCoverageModel(PolicyCoverage coverage)
        {
            var coverageModel = new CoverageModel();

            coverageModel.CoverageId = coverage.PolicyCoverageId;
            coverageModel.Coverage = coverage.PolicyCoverageName;
            coverageModel.Limit = coverage.Limit.HasValue ? coverage.Limit.Value : 0.00m;
            coverageModel.Deductible = coverage.Deductible.HasValue ? coverage.Deductible.Value : 0.00m;
            coverageModel.TransactionId = coverage.TransactionId;

            return coverageModel;
        }

        public static PolicyCoverage ToPolicyCoverage(CoverageModel coverageModel)
        {
            var coverage = new PolicyCoverage();

            coverage.PolicyCoverageId = coverageModel.CoverageId;
            coverage.PolicyCoverageName = !string.IsNullOrEmpty(coverageModel.Coverage) ? coverageModel.Coverage.Trim() : null;
            coverage.Limit = coverageModel.Limit > 0.00m ? coverageModel.Limit : (decimal?) null;
            coverage.Deductible = coverageModel.Deductible > 0.00m ? coverageModel.Deductible : (decimal?) null;
            coverage.TransactionId = coverageModel.TransactionId;

            return coverage;
        }
    }
}
