// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Defines a system that can extract a <see cref="ProjectionKey" /> from a json.
    /// </summary>
    public interface IProjectionKeyPropertyExtractor
    {
        /// <summary>
        /// Try to extract <see cref="ProjectionKey" /> from string.
        /// </summary>
        /// <param name="jsonString">The json string to extract key from.</param>
        /// <param name="keySelectorExpression">The key selector expression</param>
        /// <param name="key">The extracted key.</param>
        /// <returns>A value indicating whether the key was extracted.</returns>
        bool TryExtract(string jsonString, KeySelectorExpression keySelectorExpression, out ProjectionKey key);
    }
}
