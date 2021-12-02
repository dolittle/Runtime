// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents the runtime representation of the embedding registration arguments.
/// </summary>
/// <param name="ExecutionContext">The execution context.</param>
/// <param name="EmbeddingDefinition">The embedding definition.</param>
public record EmbeddingRegistrationArguments(ExecutionContext ExecutionContext, EmbeddingDefinition Definition);