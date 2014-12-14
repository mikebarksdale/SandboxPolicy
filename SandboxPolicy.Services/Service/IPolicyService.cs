using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Models;

namespace SandboxPolicy.Services
{
    public interface IPolicyService
    {
        PolicyModel CreatePolicy(PolicyModel policyModel);
        PolicyModel UpdatePolicy(PolicyModel policyModel);
        PolicyModel CopyQuote(int policyId, int mod);
        PolicyModel IssueQuote(PolicyModel policyModel);

        void RenewPolicy();
        void GetPolicies();
        void GetQuotesForPolicy();
    }
}
