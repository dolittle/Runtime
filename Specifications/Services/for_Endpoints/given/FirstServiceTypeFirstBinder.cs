// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Services.for_Endpoints.given
{
    public class FirstServiceTypeFirstBinder : ICanBindFirstServiceType
    {
        public IEnumerable<Service> ServicesToBind;

        public ServiceAspect Aspect => "Specs";

        public IEnumerable<Service> BindServices() => ServicesToBind;
    }
}