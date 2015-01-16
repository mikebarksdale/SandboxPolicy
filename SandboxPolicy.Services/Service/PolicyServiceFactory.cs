using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using SandboxPolicy.Repository;

namespace SandboxPolicy.Services.Service
{
    public interface IPolicyServiceFactory
    {
        IPolicyService GetService(Type type);
    }

    public class PolicyServiceFactory : IPolicyServiceFactory
    {
        public IPolicyService GetService(Type type)
        {
            if(type == typeof(PolicyDbService))
                return new PolicyDbService(new SandboxPolicyEntities());
            
            if(type == typeof(PolicyUnitOfWorkService))
                return new PolicyUnitOfWorkService(new UnitOfWork(new SandboxPolicyEntities()));

            throw new ArgumentException(string.Format("Type {0} not recognized", type));
        }
    }
}
