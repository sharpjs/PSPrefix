// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Commands;

/// <summary>
///   The <c>Get-SynchronizedHost</c> command.
///   Gets a thread-safe object that represents the current host program.
/// </summary>
[Cmdlet(VerbsCommon.Get, "SynchronizedHost", ConfirmImpact = ConfirmImpact.Low)]
[OutputType(typeof(SynchronizedHost))]
public class GetSynchronizedHostCommand : PSCmdlet
{
    /// <inheritdoc/>
    protected override void ProcessRecord()
    {
        WriteObject(new SynchronizedHost(Host));
    }
}
