// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;

namespace Dolittle.Runtime.Services.Callbacks
{
    public interface ICallbackMetricsCollector
    {

        void IncrementTotalCallbacksRegistered();
        void IncrementTotalCallbacksCalled();
        void AddToTotalCallbackTime(TimeSpan elapsed);
        void IncrementTotalCallbacksFailed();
        void IncrementTotalCallbacksUnregistered();
        void IncrementTotalSchedulesMissed();
        void AddToTotalSchedulesMissedTime(TimeSpan elapsed);
        void IncrementTotalCallbackLoopsFailed();
    }
}
