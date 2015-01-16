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
        IPolicyModel CreatePolicy(IPolicyModel policyModel);
        IPolicyModel UpdatePolicy(IPolicyModel policyModel);
        IPolicyModel CopyQuote(int policyId, int mod);
        IPolicyModel IssueQuote(IPolicyModel policyModel);

        void RenewPolicy();
        void GetPolicies();
        void GetQuotesForPolicy();
    }
}
