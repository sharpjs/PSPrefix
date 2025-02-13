// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

internal class StringPrefix : SimplePrefix
{
    private readonly string _prefix;

    public StringPrefix(string prefix)
    {
        if (prefix is null)
            throw new ArgumentNullException(nameof(prefix));

        ForegroundColor = ConsoleColor.DarkBlue;

        _prefix = $"[{prefix}] ";
    }

    protected internal override string GetPrefix() => _prefix;
}
