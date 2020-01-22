// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Moq;

namespace Dolittle.Runtime.Events
{
    public static class mocks
    {
        public static Mock<ILogger> a_logger()
        {
            var logger = new Mock<ILogger>();
            return logger;
        }
    }
}