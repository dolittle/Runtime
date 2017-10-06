/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using doLittle.Collections;
using doLittle.Execution;
using doLittle.Logging;
using doLittle.Types;

namespace doLittle.Applications
{
    /// <summary>
    /// Represents an implementation of <see cref="IApplicationResourceResolver"/>
    /// </summary>
    [Singleton]
    public class ApplicationResourceResolver : IApplicationResourceResolver
    {
        IApplication _application;
        IApplicationResourceTypes _types;
        ITypeFinder _typeFinder;
        ILogger _logger;
        Dictionary<string, ICanResolveApplicationResources> _resolversByType;

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationResourceResolver"/>
        /// </summary>
        /// <param name="application">Current <see cref="IApplication">Application</see></param>
        /// <param name="types"><see cref="IApplicationResourceTypes">Resource types</see> available</param>
        /// <param name="resolvers">Instances of <see cref="ICanResolveApplicationResources"/> for specialized resolving</param>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> for discovering types needed</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public ApplicationResourceResolver(
            IApplication application, 
            IApplicationResourceTypes types, 
            IInstancesOf<ICanResolveApplicationResources> resolvers, 
            ITypeFinder typeFinder,
            ILogger logger)
        {
            _application = application;
            _types = types;
            _resolversByType = resolvers.ToDictionary(r => r.ApplicationResourceType.Identifier, r => r);
            _typeFinder = typeFinder;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Type Resolve(IApplicationResourceIdentifier identifier)
        {
            _logger.Trace($"Trying to resolve : {identifier.Resource.Name} - with type {identifier.Resource.Type.Identifier}");

            var typeIdentifier = identifier.Resource.Type.Identifier;
            if (_resolversByType.ContainsKey(typeIdentifier)) return _resolversByType[typeIdentifier].Resolve(identifier);

            var resourceType = _types.GetFor(typeIdentifier);
            var types = _typeFinder.FindMultiple(resourceType.Type);
            var typesMatchingName = types.Where(t => t.Name == identifier.Resource.Name);

            ThrowIfAmbiguousTypes(identifier, typesMatchingName);

            var formats = _application.Structure.GetStructureFormatsForArea(resourceType.Area);
            var type = typesMatchingName.Where(t => formats.Any(f => f.Match(t.Namespace).HasMatches)).FirstOrDefault();
            if (type != null) return type;

            _logger.Error($"Unknown application resurce type : {identifier.Resource.Type.Identifier}");
            throw new UnknownApplicationResourceType(identifier.Resource.Type.Identifier);
        }

        void ThrowIfAmbiguousTypes(IApplicationResourceIdentifier identifier, IEnumerable<Type> typesMatchingName)
        {
            if (typesMatchingName.Count() > 1) 
            {
                _logger.Error($"Ambiguous types found for {identifier.Resource.Name}");
                typesMatchingName.ForEach(type => _logger.Trace($"  Type found: {type.AssemblyQualifiedName}"));
                throw new AmbiguousTypes(identifier);
            }
        }
    }
}
