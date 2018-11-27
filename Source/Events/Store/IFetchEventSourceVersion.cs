namespace Dolittle.Runtime.Events.Store
{
    using Dolittle.Events;

    /// <summary>
    /// Defines the ability to fetch the current <see cref="EventSourceVersion" /> for a specific <see cref="EventSource" />
    /// </summary>
    public interface IFetchEventSourceVersion
    {
         /// <summary>
         /// Returns the current <see cref="EventSourceVersion" /> for a specific <see cref="EventSource" />
         /// </summary>
         /// <param name="eventSource">The <see cref="EventSourceKey" /> to get the verison for</param>
         /// <returns>The <see cref="EventSourceVersion" /> for this <see cref="EventSource" />, <see cref="EventSourceVersion.NoVersion" /> if the <see cref="EventSource" /> has not been persisted before</returns>
         EventSourceVersion GetCurrentVersionFor(EventSourceKey eventSource);
         
         /// <summary>
         /// Returns the next <see cref="EventSourceVersion" /> for a specific <see cref="EventSource" />
         /// </summary>
         /// <param name="eventSource">The <see cref="EventSourceKey" /> to get the next verison for</param>
         /// <returns>The next <see cref="EventSourceVersion" /> for this <see cref="EventSource" />, the  <see cref="EventSourceVersion.Initial" /> if the <see cref="EventSource" /> has not been persisted before</returns>
         EventSourceVersion GetNextVersionFor(EventSourceKey eventSource);
    }
}