// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents an implementation of <see cref="IValidateOccurredFormat"/>.
/// </summary>
public class ValidateOccurredFormat : IValidateOccurredFormat
{
    public bool IsValid(OccurredFormat format, out Exception error)
    {
        try
        {
            error = null;
            if (string.IsNullOrEmpty(format))
            {
                error = new OccurredFormatCannotBeNullOrEmpty();
                return false;
            }
            _ = DateTimeOffset.UtcNow.ToString(format, CultureInfo.InvariantCulture);
            return true;
        }
        catch (Exception ex)
        {
            error = ex;
            return false;
        }
    }
}
