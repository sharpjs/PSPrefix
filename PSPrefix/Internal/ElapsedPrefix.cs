// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Diagnostics;

namespace PSPrefix.Internal;

/// <summary>
///   An elapsed-time prefix for <see cref="PrefixedHost"/>.
/// </summary>
internal class ElapsedPrefix : SimplePrefix
{
    // Format:
    // -   [+hh:mm:ss] when <  1 day
    // - [+d.hh:mm:ss] when >= 1 day

    private readonly Stopwatch _stopwatch;

    private TimeSpan _cachedElapsed;
    private string?  _cachedPrefix;

    /// <summary>
    ///   Initializes a new <see cref="ElapsedPrefix"/> instance that counts
    ///   elapsed time from the current moment.
    /// </summary>
    public ElapsedPrefix()
    {
        _stopwatch     = Stopwatch.StartNew();
        _cachedElapsed = TimeSpan.MinValue;
    }

    /// <inheritdoc/>
    protected internal override string GetPrefix()
    {
        var elapsed = Floor(_stopwatch.Elapsed);

        return elapsed == _cachedElapsed
            ? _cachedPrefix!
            : _cachedPrefix = $@"[+{Floor(_cachedElapsed = elapsed):c}] ";
    }

    internal static TimeSpan Floor(TimeSpan elapsed)
    {
        return elapsed - new TimeSpan(elapsed.Ticks % TimeSpan.TicksPerSecond);
    }
}
