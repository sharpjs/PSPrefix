// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Commands;

/// <summary>
///   The <c>Show-Prefixed</c> command.
/// </summary>
[Cmdlet(VerbsCommon.Show, "Prefixed", ConfirmImpact = ConfirmImpact.Low)]
public class ShowPrefixedCommand : InvokeScriptBlockCommand
{
    private PrefixedHost? _host;

    /// <summary>
    ///   <b>-Prefix:</b>
    ///   TODO
    /// </summary>
    [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
    [ValidateNotNullOrEmpty]
    public string? Prefix { get; set; }

    /// <inheritdoc/>
    protected internal override PSHost EffectiveHost => _host ?? base.EffectiveHost;

    /// <inheritdoc/>
    protected override void ProcessRecord()
    {
        _host = new PrefixedHost(base.EffectiveHost, new StringPrefix(Prefix!));

        base.ProcessRecord();
    }
}
