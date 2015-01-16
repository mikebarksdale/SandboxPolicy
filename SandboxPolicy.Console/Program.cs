using System;
using System.Linq;
using Microsoft.Practices.Unity;
using SandboxPolicy.Models;
using SandboxPolicy.Services;
using SandboxPolicy.Services.Service;

namespace SandboxPolicy.Console
{
    class Program
    {
        protected static IUnityContainer Container;
        protected static IPolicyServiceFactory PolicyServiceFactory;

        static void Main(string[] args)
        {
            InitializeContainer();
            PolicyServiceFactory = Container.Resolve<IPolicyServiceFactory>();

            IPolicyModel policyModel = new PolicyModel
            {
                EffectiveDate = new DateTime(2014, 12, 25),
                ExpirationDate = new DateTime(2015, 12, 25),
                User = "testmb"
            };

            //create a quote
            policyModel = CreatePolicyAction(policyModel);
            
            //insured view
            var insuredModel = new InsuredModel
            {
                FirstName = "John",
                LastName = "Doe",
                InsuredType = "Primary"
            };

            policyModel.Insureds.Add(insuredModel);
            policyModel = UpdatePolicyAction(policyModel);
            
            //coverage view
            var coverageModel = new CoverageModel
            {
                Coverage = "Liability",
                Limit = 1000000m
            };

            policyModel.Coverages.Add(coverageModel);
            policyModel = UpdatePolicyAction(policyModel);

            //before issuing, let's make a few copies and make some changes
            var quoteModel1 = CopyQuote(policyModel);
            var quoteModel2 = CopyQuote(policyModel);

            quoteModel1.Description = "2mil liability";
            quoteModel1.Coverages.First().Limit = 2000000m;

            quoteModel2.Description = "1mil liab + 25000 ded + TRIA";
            quoteModel2.Coverages.First().Deductible = 25000m;

            quoteModel2.Coverages.Add(new CoverageModel
            {
                Coverage = "Terrorism Coverage",
                Limit = 500000m,
                Deductible = 10000m
            });

            quoteModel1 = UpdatePolicyAction(quoteModel1);
            quoteModel2 = UpdatePolicyAction(quoteModel2);

            //issue the second quote
            quoteModel1.User = "testmb";
            IssuePolicyAction(quoteModel1);
        }

        private static void InitializeContainer()
        {
            Container = new UnityContainer();
            Container.RegisterType<IPolicyServiceFactory, PolicyServiceFactory>();
        }

        private static IPolicyModel CreatePolicyAction(IPolicyModel policyModel)
        {
            var policyService = PolicyServiceFactory.GetService(typeof (PolicyDbService));
            return policyService.CreatePolicy(policyModel);
        }

        private static IPolicyModel UpdatePolicyAction(IPolicyModel policyModel)
        {
            var policyService = PolicyServiceFactory.GetService(typeof(PolicyDbService));
            return policyService.UpdatePolicy(policyModel);
        }

        private static IPolicyModel IssuePolicyAction(IPolicyModel policyModel)
        {
            var policyService = PolicyServiceFactory.GetService(typeof(PolicyDbService));
            return policyService.IssueQuote(policyModel);
        }

        private static IPolicyModel CopyQuote(IPolicyModel policyModel)
        {
            var policyService = PolicyServiceFactory.GetService(typeof(PolicyDbService));
            return policyService.CopyQuote(policyModel.PolicyId, policyModel.Mod);
        }
    }
}
