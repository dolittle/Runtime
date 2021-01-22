// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Moq;

namespace Dolittle.Runtime.Rules.Machine.Specifications
{
    /// <summary>
    /// Represents assertions related to <see cref="IRuleContext"/>.
    /// </summary>
    public static class RuleContextAssertions
    {
        /// <summary>
        /// Asserts that a <see cref="Mock{T}"/> of <see cref="IRuleContext"/> never has any failures
        /// added to it.
        /// </summary>
        /// <param name="ruleContextMock"><see cref="Mock{T}"/> of <see cref="IRuleContext"/>.</param>
        public static void ShouldNotFail(this Mock<IRuleContext> ruleContextMock)
        {
            ruleContextMock.Verify(r => r.Fail(Moq.It.IsAny<IRule>(), Moq.It.IsAny<object>(), Moq.It.IsAny<Cause>()), Times.Never());
        }

        /// <summary>
        /// Asserts that a <see cref="Mock{T}"/> of <see cref="IRuleContext"/> fails with specific
        /// <see cref="IRule"/>, instance and <see cref="Reason"/>.
        /// </summary>
        /// <param name="ruleContextMock"><see cref="Mock{T}"/> of <see cref="IRuleContext"/>.</param>
        /// <param name="rule"><see cref="IRule"/> it should fail with.</param>
        /// <param name="instance">Instance it should fail with.</param>
        /// <param name="reason"><see cref="Reason"/> it should fail with.</param>
        public static void ShouldFailWith(this Mock<IRuleContext> ruleContextMock, IRule rule, object instance, Reason reason)
        {
            ruleContextMock.Verify(r => r.Fail(rule, instance, Moq.It.Is<Cause>(_ => _.Reason == reason)), Times.Once());
        }
    }
}