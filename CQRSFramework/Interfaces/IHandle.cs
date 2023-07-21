using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSFramework
{
    public interface IHandle<T> where T : DomainEvent
    {
        void Handle(T args);
    }
}
