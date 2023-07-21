using CQRSFramework.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CQRSFramework
{
    public interface IRepository<T, TId> where T : AggregateRoot<TId>
    {
        Task CommitChanges(DomainEvent @event);
        Task<T> RehydrateAggregateFromEventStream();
        Task LogAsync(string msg);
        Task ClearLogAsync();
    }
}
