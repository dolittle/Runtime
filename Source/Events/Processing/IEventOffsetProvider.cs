namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventOffsetProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        void Set(EventProcessorIdentifier id, ulong offset);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ulong Get(EventProcessorIdentifier id);
    }
}