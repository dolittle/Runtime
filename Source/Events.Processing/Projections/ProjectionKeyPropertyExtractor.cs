// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Serialization.Json;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionKeyPropertyExtractor" />.
    /// </summary>
    public class ProjectionKeyPropertyExtractor : IProjectionKeyPropertyExtractor
    {
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes an instance of the <see cref="ProjectionKeyPropertyExtractor" /> class.
        /// </summary>
        /// <param name="serializer">The json serializer.</param>
        public ProjectionKeyPropertyExtractor(ISerializer serializer)
        {
            _serializer = serializer;
        }

        /// <inheritdoc/>
        public bool TryExtract(string jsonString, KeySelectorExpression keySelectorExpression, out ProjectionKey key)
        {
            key = null;
            var contentKeyValues = _serializer.GetKeyValuesFromJson(jsonString);
            if (!contentKeyValues.ContainsKey(keySelectorExpression)) return false;

            key = AsProjectionKey(contentKeyValues[keySelectorExpression]);
            return true;
        }

        ProjectionKey AsProjectionKey(object key)
            => key.ToString(); // TODO: This should perhaps be more sophisticated.
    }
}
