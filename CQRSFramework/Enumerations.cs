using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSFramework
{
    public enum CommandType
    {
        None = 0,
        ScheduleAppointment = 1,
        CancelApoointment = 2,
        CreateNewDeclaration = 3,
        UpdateDeclaration = 4
    }

    public enum DomainEventType
    {
        Batch = 0,
        DeclarationCreated = 1,
        DeclarationUpdated = 2,
        IncidentCreated = 3,
        IncidentUpdated = 4,
        IncidentDeleted = 5,
        AppointmentScheduled = 6,
        AppointmentCancelled = 7
    }

    public enum TypeOfModification
    {
        None = 0,
        Create = 1,
        Edit = 2,
        Delete = 3
    }
}
