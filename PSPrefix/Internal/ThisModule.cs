// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

/// <summary>
///   Represents the PSPrefix PowerShell module.
/// </summary>
internal abstract class ThisModule
{
    [ExcludeFromCodeCoverage] // uninvokable
    private ThisModule() { }

    /// <summary>
    ///   Gets the version of the module.
    /// </summary>
    public static Version Version { get; }
        = typeof(ThisModule).Assembly.GetName().Version!;
}
