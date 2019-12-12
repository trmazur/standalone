using Prism.Events;
using VMS.TPS.Common.Model.API;

namespace Model_Validation.Events
{
    public class PatientSelectedEvent:PubSubEvent<Patient>
    {
    }
}
