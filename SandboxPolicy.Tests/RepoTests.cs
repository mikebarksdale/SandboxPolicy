using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SandboxPolicy.Models;
using SandboxPolicy.Models.Fakes;
using SandboxPolicy.Services;
using SandboxPolicy.Services.Service;

namespace SandboxPolicy.Tests
{
    [TestClass]
    public class RepoTests
    {
        protected static IUnityContainer Container;
        protected static IPolicyServiceFactory PolicyServiceFactory;

        [TestInitialize]
        public void Initialize()
        {
            Container = new UnityContainer();
            Container.RegisterType<IPolicyServiceFactory, PolicyServiceFactory>();

            PolicyServiceFactory = Container.Resolve<IPolicyServiceFactory>();
        }

        [TestMethod]
        public void CreatePolicyDbPass()
        {
            var policyModel = new StubIPolicyModel
            {
                EffectiveDateGet = () => new DateTime(2014, 12, 25),
                ExpirationDateGet = () => new DateTime(2015, 12, 25),
                InsuredsGet = () => new List<IInsuredModel>(),
                CoveragesGet = () => new List<ICoverageModel>(),
                UserGet = () => "testmb"
            };

            var service = PolicyServiceFactory.GetService(typeof (PolicyDbService));
            var policy = service.CreatePolicy(policyModel);

            Assert.IsNotNull(policy.PolicyNumber);
            Assert.IsTrue(policy.PolicyId > 0);
            Assert.IsTrue(policy.Status.Equals("Quote", StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void CreatePolicyAndUpdateWithInsuredsPass()
        {
            var policyModel = new StubIPolicyModel
            {
                EffectiveDateGet = () => new DateTime(2014, 12, 25),
                ExpirationDateGet = () => new DateTime(2015, 12, 25),
                InsuredsGet = () => new List<IInsuredModel>(),
                CoveragesGet = () => new List<ICoverageModel>(),
                UserGet = () => "testmb"
            };

            var insuredModel = new StubIInsuredModel
            {
                FirstNameGet = () => "Peter",
                LastNameGet = () => "Quill",
                DbaNameGet = () => "Star Lord",
                InsuredTypeGet = () => "Primary"
            };

            var service = PolicyServiceFactory.GetService(typeof (PolicyDbService));
            var policy = service.CreatePolicy(policyModel);

            policy.Insureds.Add(insuredModel);

            policy = service.UpdatePolicy(policy);

            Assert.IsTrue(policy.Insureds.First().InsuredId > 0);
        }
    }
}
