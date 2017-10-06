/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace doLittle.Events
{
    /// <summary>
    /// Defines the coordinator that coordinates <see cref="CommittedEventStream">committed event streams</see>
    /// </summary>
    public interface ICommittedEventStreamCoordinator
    {
        /// <summary>
        /// Initialize the coordinator
        /// </summary>
        void Initialize();
    }
}
