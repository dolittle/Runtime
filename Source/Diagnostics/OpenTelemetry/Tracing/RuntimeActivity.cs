// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Diagnostics.OpenTelemetry;

public static class RuntimeActivity
{
    public const string SourceName = "Dolittle.Runtime";
    public static readonly System.Diagnostics.ActivitySource Source = new(SourceName);
}
