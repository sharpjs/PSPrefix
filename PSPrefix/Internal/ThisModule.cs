// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

internal abstract class ThisModule
{
    [ExcludeFromCodeCoverage] // uninvokable
    private ThisModule() { }

    public static Version Version { get; }
        = typeof(ThisModule).Assembly.GetName().Version!;
}
