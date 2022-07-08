// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Holds the unique <see cref="FailureId"> failure ids </see> unique to the Embeddings.
/// </summary>
public static class EmbeddingFailures
{
    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'NoEmbeddingRegistrationReceived' failure type.
    /// </summary>
    public static FailureId NoEmbeddingRegistrationReceived => FailureId.Create("bda8fb78-1cbb-4b1b-962c-eb8f90087bb0");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'NoEmbeddingRegisteredForTenant' failure type.
    /// </summary>s
    public static FailureId NoEmbeddingRegisteredForTenant => FailureId.Create("d76e5522-9bbd-4bb1-87df-b167e42acf02");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'FailedToRegisterEmbedding' failure type.
    /// </summary>
    public static FailureId FailedToRegisterEmbedding => FailureId.Create("77cd99ec-9939-43ae-83dc-3c770ef65a4c");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'FailedToUpdateEmbedding' failure type.
    /// </summary>
    public static FailureId FailedToUpdateEmbedding => FailureId.Create("4f765146-c635-497a-a14b-b7f80d5cf875");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'FailedToDeleteEmbedding' failure type.
    /// </summary>
    public static FailureId FailedToDeleteEmbedding => FailureId.Create("dcfba172-9348-4b2c-b518-17603798c0ec");
}