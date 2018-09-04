#pragma warning disable 1591

namespace Dolittle.Runtime.Events.Processing
{
    using Dolittle.Runtime.Tenancy;

    /// <summary>
    /// 
    /// </summary>
    public interface IScopedEventOffsetProvider 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenant"></param>
        /// <param name="offset"></param>
        void Set(EventProcessorId id, TenantId tenant, ulong offset);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        ulong Get(EventProcessorId id, TenantId tenant);
    }
}