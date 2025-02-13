// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

internal abstract class SimplePrefix : IPrefix
{
    public int Length => GetPrefix().Length;

    public ConsoleColor ForegroundColor { get; set; }
        = ConsoleColor.DarkGray;

    public ConsoleColor BackgroundColor { get; set; }
        = ConsoleColor.Black;

    public void Write(PSHostUserInterface ui)
    {
        ui.Write(ForegroundColor, BackgroundColor, GetPrefix());
    }

    protected internal abstract string GetPrefix();
}
