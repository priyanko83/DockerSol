using CQRSFramework.Commands.Brokers;
using System.Threading.Tasks;

namespace CQRSFramework.CommandHandlers.Brokers
{
    public class AppointmentCommandHandlers :
        ICommandHandler<CancelAppointment>,
        ICommandHandler<ScheduleAppointment>
    {
        async Task ICommandHandler<CancelAppointment>.HandleAsync(CancelAppointment command)
        {
            
        }

        async Task ICommandHandler<ScheduleAppointment>.HandleAsync(ScheduleAppointment command)
        {
            
        }
    }
}
