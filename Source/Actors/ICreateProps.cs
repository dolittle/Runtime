// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Proto;

namespace Dolittle.Runtime.Actors;

public interface ICreateProps
{
    Props PropsFor<TActor>(params object[] parameters)
        where TActor : IActor;
}
