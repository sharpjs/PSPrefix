// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

/// <summary>
///   A prefix for <see cref="PrefixedHost"/>.
/// </summary>
internal interface IPrefix
{
    /// <summary>
    ///   Gets the length of the prefix in UTF-16 code units.
    /// </summary>
    int Length { get; }

    /// <summary>
    ///   Writes the prefix to the specified PowerShell host user interface.
    /// </summary>
    /// <param name="ui">
    ///   The PowerShell host user interface to which to write the prefix.
    /// </param>
    void Write(PSHostUserInterface ui);
}
