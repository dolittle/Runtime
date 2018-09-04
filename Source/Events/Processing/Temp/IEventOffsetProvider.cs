#pragma warning disable 1591

namespace Dolittle.Runtime.Events.Processing
{
    public interface IEventOffsetProvider
    {
        void Set(EventProcessorId id, ulong offset);
        ulong Get(EventProcessorId id);
    }
}