// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the unique identifier of an event store failure type.
    /// </summary>
    public class FiltersFailureId : FailureId
    {
        /// <summary>
        /// Gets the <see cref="FiltersFailureId" /> that represents the 'NoFilterRegistration' failure type.
        /// </summary>
        public static FiltersFailureId NoFilterRegistration => new FiltersFailureId { Value = Guid.Parse("d6060ba0-39bd-4815-8b0e-6b43b5f87bc5") };

        /// <summary>
        /// Gets the <see cref="FiltersFailureId" /> that represents the 'CannotRegisterFilterOnNonWriteableStream' failure type.
        /// </summary>
        public static FiltersFailureId CannotRegisterFilterOnNonWriteableStream => new FiltersFailureId { Value = Guid.Parse("2cdb6143-4f3d-49cb-bd58-68fd1376dab1") };

        /// <summary>
        /// Gets the <see cref="FiltersFailureId" /> that represents the 'FailedToRegisterFilter' failure type.
        /// </summary>
        public static FiltersFailureId FailedToRegisterFilter => new FiltersFailureId { Value = Guid.Parse("f0480899-8aed-4191-b339-5121f4d9f2e2") };
    }
}
