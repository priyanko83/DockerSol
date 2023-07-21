using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSFramework
{
    public interface ICommandHandler<T> where T : ApplicationCommand
    {
        Task HandleAsync(T command);
    }
}
