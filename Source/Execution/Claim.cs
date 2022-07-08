// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Execution;

/// <summary>
/// Represents a Claim.
/// </summary>
public record Claim(
    string Name, 
    string Value, 
    string ValueType);
