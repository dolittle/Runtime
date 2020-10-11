// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Moq;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class MockExtensions
    {
        public static void VerifyOnlyCall<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression)
            where T : class
        {
            mock.Verify(expression, Times.Once);
            mock.VerifyNoOtherCalls();
        }
    }
}
