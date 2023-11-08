// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;

namespace Dolittle.Runtime.Events.Store.MongoDB;

public static partial class StreamIdMatcher
{
    public static bool IsMatch(string input)
    {
        return StreamIdRegex().IsMatch(input);
    }

    [GeneratedRegex("^stream-[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex StreamIdRegex();
}
