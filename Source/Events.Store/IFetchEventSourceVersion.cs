namespace Dolittle.Runtime.Events.Store
{
    using Dolittle.Events;

    /// <summary>
    /// Defines the ability to fetch the current <see cref="EventSourceVersion" /> for a specific <see cref="EventSource" />
    /// </summary>
    public interface IFetchEventSourceVersion
    {
         /// <summary>
         /// Returns the latest <see cref="EventSourceVersion" /> for a specific <see cref="EventSource" />
         /// </summary>
         /// <param name="eventSource">The <see cref="EventSourceId" /> to get the verison for</param>
         /// <returns>The <see cref="EventSourceVersion" /> for this <see cref="EventSource" /></returns>
         EventSourceVersion GetVersionFor(EventSourceId eventSource);
    }
}