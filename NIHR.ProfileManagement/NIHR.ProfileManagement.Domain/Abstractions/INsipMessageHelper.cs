
using NIHR.ProfileManagement.Domain.Models;

namespace NIHR.ProfileManagement.Domain.Abstractions
{
    public interface INsipMessageHelper
    {
        NsipMessage<string> Prepare(string eventType, string sourceSystem, string eventData);
    }
}
