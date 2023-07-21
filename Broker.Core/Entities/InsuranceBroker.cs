using CQRSFramework;
using System;

namespace Brokers.Core.Entities
{
    public class InsuranceBroker : Entity<Guid>
    {
        public string BrokerName { get; private set; }
        
        public InsuranceBroker(string name, Guid id) : base(id)
        {
            this.BrokerName = name;
        }
    }
}
