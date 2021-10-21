// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.Runtime.Protobuf;
namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers
{
    /// <summary>
    /// Defines a system that can convert an exception to a failure.
    /// </summary>
    public interface IExceptionToFailureConverter
    {
        Failure ToFailure(Exception exception);
    }
}