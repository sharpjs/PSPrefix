// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Diagnostics;

namespace PSPrefix.Internal;

internal class ElapsedPrefix : SimplePrefix
{
    // Format:
    // -   [+hh:mm:ss] when <  1 day
    // - [+d.hh:mm:ss] when >= 1 day

    private readonly Stopwatch _stopwatch;

    private TimeSpan _cachedElapsed;
    private string?  _cachedPrefix;

    public ElapsedPrefix()
    {
        _stopwatch     = Stopwatch.StartNew();
        _cachedElapsed = TimeSpan.MinValue;
    }

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
