// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Commands;

/// <summary>
///   The New-SynchronizedHost command.
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
