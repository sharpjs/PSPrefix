// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

/// <summary>
///   A prefix for <see cref="PrefixedHost"/> consisting of a string written
///   with one foreground-background color pair.
/// </summary>
internal abstract class SimplePrefix : IPrefix
{
    /// <inheritdoc/>
    public int Length => GetPrefix().Length;

    /// <summary>
    ///   Gets or sets the foreground color of the prefix.
    ///   The default is <see cref="ConsoleColor.DarkGray"/>.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.DarkGray;

    /// <summary>
    ///   Gets or sets the background color of the prefix.
    ///   The default is <see cref="ConsoleColor.Black"/>.
    /// </summary>
    public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

    /// <inheritdoc/>
    public void Write(PSHostUserInterface ui)
    {
        ui.Write(ForegroundColor, BackgroundColor, GetPrefix());
    }

    /// <summary>
    ///   Gets the string value of the prefix.
    /// </summary>
    protected internal abstract string GetPrefix();
}
