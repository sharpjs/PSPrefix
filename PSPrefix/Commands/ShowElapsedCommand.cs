// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Commands;

/// <summary>
///   The <c>Show-Elapsed</c> command.
/// </summary>
[Cmdlet(VerbsCommon.Show, "Elapsed", ConfirmImpact = ConfirmImpact.Low)]
public class ShowElapsedCommand : InvokeScriptBlockCommand
{
    private PrefixedHost? _host;

    /// <inheritdoc/>
    protected internal override PSHost EffectiveHost => _host ?? base.EffectiveHost;

    /// <inheritdoc/>
    protected override void BeginProcessing()
    {
        _host = new PrefixedHost(base.EffectiveHost, new ElapsedPrefix());
    }
}
