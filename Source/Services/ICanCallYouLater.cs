// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services
{
    public interface ICanCallYouLater
    {
        IDisposable TryRegisterCallback(Action callback, TimeSpan delay);
    }
}
