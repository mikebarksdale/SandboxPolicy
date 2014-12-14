using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxPolicy.Entities;
using SandboxPolicy.Models;

namespace SandboxPolicy.Services.Adapters
{
    public static class InsuredAdapter
    {
        public static InsuredModel ToInsuredModel(Insured insured)
        {
            var insuredModel = new InsuredModel();

            insuredModel.InsuredId = insured.InsuredId;
            insuredModel.FirstName = insured.FirstName;
            insuredModel.MiddleName = insured.MiddleName;
            insuredModel.LastName = insured.LastName;
            insuredModel.DbaName = insured.DbaName;
            insuredModel.InsuredType = insured.InsuredType;
            insuredModel.TransactionId = insured.TransactionId;

            return insuredModel;
        }

        public static Insured ToInsured(InsuredModel insuredModel)
        {
            var insured = new Insured();

            insured.InsuredId = insuredModel.InsuredId;
            insured.FirstName = !string.IsNullOrEmpty(insuredModel.FirstName) ? insuredModel.FirstName.Trim() : null;
            insured.MiddleName = !string.IsNullOrEmpty(insuredModel.MiddleName) ? insuredModel.MiddleName.Trim() : null;
            insured.LastName = !string.IsNullOrEmpty(insuredModel.LastName) ? insuredModel.LastName.Trim() : null;
            insured.DbaName = !string.IsNullOrEmpty(insuredModel.DbaName) ? insuredModel.DbaName.Trim() : null;
            insured.InsuredType = insuredModel.InsuredType;
            insured.TransactionId = insuredModel.TransactionId;

            return insured;
        }
    }
}
