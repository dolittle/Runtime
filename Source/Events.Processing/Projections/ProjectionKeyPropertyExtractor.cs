// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionKeyPropertyExtractor" />.
    /// </summary>
    public class ProjectionKeyPropertyExtractor : IProjectionKeyPropertyExtractor
    {
        /// <inheritdoc/>
        public bool TryExtract(string jsonString, KeySelectorExpression keySelectorExpression, out ProjectionKey key)
        {
            key = null;
            var jsonObject = JObject.Parse(jsonString);
            var property = jsonObject.SelectToken(keySelectorExpression, false);
            if (property == null) return false;

            key = AsProjectionKey(property);
            return true;
        }

        ProjectionKey AsProjectionKey(object key)
            => key.ToString(); // TODO: This should perhaps be more sophisticated.
    }
}
