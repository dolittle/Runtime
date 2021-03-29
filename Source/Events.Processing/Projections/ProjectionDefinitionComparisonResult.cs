// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents the result of the comparison of a <see cref="ProjectionDefinition" />.
    /// </summary>
    public class ProjectionDefinitionComparisonResult
    {
        /// <summary>
        /// Creates a successful <see cref="ProjectionDefinitionComparisonResult" />.
        /// </summary>
        /// <returns>The successful <see cref="ProjectionDefinitionComparisonResult" />.</returns>
        public static ProjectionDefinitionComparisonResult Equal => new();

        /// <summary>
        /// Creates a failed <see cref="ProjectionDefinitionComparisonResult" />.
        /// </summary>
        /// <param name="reason">The reason why they're unequal.</param>
        /// <returns>The failed <see cref="ProjectionDefinitionComparisonResult" />.</returns>
        public static ProjectionDefinitionComparisonResult Unequal(FailedProjectionDefinitionComparisonReason reason)
            => new(reason);

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionDefinitionComparisonResult"/> class.
        /// </summary>
        ProjectionDefinitionComparisonResult() => Succeeded = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionDefinitionComparisonResult"/> class.
        /// </summary>
        /// <param name="reason">The <see cref="FailedProjectionDefinitionComparisonReason" />.</param>
        ProjectionDefinitionComparisonResult(FailedProjectionDefinitionComparisonReason reason) => FailureReason = reason;

        /// <summary>
        /// Gets a value indicating whether the validation succeeded or not.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the <see cref="FailedProjectionDefinitionComparisonReason" />.
        /// </summary>
        public FailedProjectionDefinitionComparisonReason FailureReason { get; }
    }
}
