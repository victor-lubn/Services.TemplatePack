using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lueben.Templates.Eventing.Clients.Stub.Models;

namespace Lueben.Templates.Eventing.Clients.Stub.Stubs
{
    public class MicroserviceDependencyClientStub : IDependencyOneClient, IDependencyTwoClient
    {
        public IEnumerable<Application> GetAllApplications()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetPIIDataForStepOne(long appId)
        {
            await Task.Delay(100);
            if (appId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(appId), "app id should be positive number.");
            }

            return 1;
        }

        public async Task ExecuteStepOne()
        {
            await Task.Delay(100);
        }

        public async Task<string> GetPIIDataForStepTwo(long appId)
        {
            await Task.Delay(100);
            return "pii data";
        }

        public async Task ExecuteStepTwo()
        {
            await Task.Delay(100);
        }
    }
}