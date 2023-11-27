// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;

namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations;

public static partial class StreamIdMatcher
{
    public static bool IsStreamOrEventLog(string input) => input.Equals("event-log") || IsStream(input) || IsScopedEventLog(input);

    public static bool IsStream(string input) => IsNormalStream(input) || IsScopedStream(input) || IsPublicStream(input);

    public static bool IsNormalStream(string input)
    {
        return StreamIdRegex().IsMatch(input);
    }
    
    public static bool IsPublicStream(string input)
    {
        return PublicStreamIdRegex().IsMatch(input);
    }

    public static bool IsScopedStream(string input)
    {
        return ScopedStreamRegex().IsMatch(input);
    }

    public static bool IsScopedEventLog(string input)
    {
        return ScopedEventLogRegex().IsMatch(input);
    }

    [GeneratedRegex("^stream-[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex StreamIdRegex();
    
    [GeneratedRegex("^public-stream-[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PublicStreamIdRegex();

    [GeneratedRegex("^x-[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}-stream-[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ScopedStreamRegex();

    [GeneratedRegex("^x-[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}-event-log$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ScopedEventLogRegex();
}
