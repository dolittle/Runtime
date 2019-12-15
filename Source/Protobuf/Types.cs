// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable CA1717, CA1720

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Represents the allowed types in an object type.
    /// </summary>
    public enum Types
    {
        /// <summary>String - UTF8 assumed</summary>
        String = 1,

        /// <summary>32 bit integer</summary>
        Int32,

        /// <summary>64 bit integer</summary>
        Int64,

        /// <summary>32 bit unsigned integer</summary>
        UInt32,

        /// <summary>64 bit unsigned integer</summary>
        UInt64,

        /// <summary>boolean</summary>
        Boolean,

        /// <summary>float 32 bit</summary>
        Float,

        /// <summary>double -  64 bit</summary>
        Double,

        /// <summary>DateTime</summary>
        DateTime,

        /// <summary>DateTimeOffset</summary>
        DateTimeOffset,

        /// <summary>A unique identifier</summary>
        Guid,

        /// <summary>Type is unknown</summary>
        Unknown
    }
}