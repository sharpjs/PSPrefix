// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

/// <summary>
///   A fixed string prefix for <see cref="PrefixedHost"/>.
/// </summary>
internal class StringPrefix : SimplePrefix
{
    private readonly string _prefix;

    /// <summary>
    ///   Initializes a new <see cref="StringPrefix"/> instance with the
    ///   specified prefix string.
    /// </summary>
    /// <param name="prefix">
    ///   The prefix string.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="prefix"/> is <see langword="null"/>.
    /// </exception>
    public StringPrefix(string prefix)
    {
        if (prefix is null)
            throw new ArgumentNullException(nameof(prefix));

        ForegroundColor = ConsoleColor.DarkBlue;

        _prefix = $"[{prefix}] ";
    }

    /// <inheritdoc/>
    protected internal override string GetPrefix() => _prefix;
}
