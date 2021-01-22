// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Versioning.for_VersionConverter.given
{
    public class a_version_converter
    {
        protected static VersionConverter version_converter;

        Establish context = () => version_converter = new VersionConverter();
    }
}