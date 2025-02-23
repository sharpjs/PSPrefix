// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Globalization;

namespace PSPrefix.Internal;

/// <summary>
///   A PowerShell host wrapper that prepends a prefix to output lines.
/// </summary>
internal sealed class PrefixedHost : PSHost
{
    private readonly PSHost          _host;
    private readonly PrefixedHostUI? _ui;
    private readonly string          _name;
    private readonly Guid            _id;

    /// <summary>
    ///   Initializes a new <see cref="PrefixedHost"/> instance.
    /// </summary>
    /// <param name="host">
    ///   The underlying host.
    /// </param>
    /// <param name="prefix">
    ///   A delegate the host can invoke to get the current prefix.
    /// </param>
    public PrefixedHost(PSHost host, IPrefix prefix)
    {
        if (host is null)
            throw new ArgumentNullException(nameof(host));
        if (prefix is null)
            throw new ArgumentNullException(nameof(prefix));

        _host = host;
        _ui   = host.UI is { } ui ? new(ui, prefix) : null;
        _name = $"{nameof(PrefixedHost)}({host.Name})";
        _id   = Guid.NewGuid();
    }

    /// <inheritdoc/>
    public override string Name
        => _name;

    /// <inheritdoc/>
    public override Version Version
        => ThisModule.Version;

    /// <inheritdoc/>
    public override Guid InstanceId
        => _id;

    /// <inheritdoc/>
    public override PSHostUserInterface? UI
        => _ui;

    /// <inheritdoc/>
    public override CultureInfo CurrentCulture
        => _host.CurrentCulture;

    /// <inheritdoc/>
    public override CultureInfo CurrentUICulture
        => _host.CurrentUICulture;

    /// <inheritdoc/>
    public override PSObject PrivateData
        => _host.PrivateData;

    /// <inheritdoc/>
    public override bool DebuggerEnabled
    {
        get => _host.DebuggerEnabled;
        set => _host.DebuggerEnabled = value;
    }

    /// <inheritdoc/>
    public override void EnterNestedPrompt()
        => _host.EnterNestedPrompt();

    /// <inheritdoc/>
    public override void ExitNestedPrompt()
        => _host.ExitNestedPrompt();

    /// <inheritdoc/>
    public override void NotifyBeginApplication()
        => _host.NotifyBeginApplication();

    /// <inheritdoc/>
    public override void NotifyEndApplication()
        => _host.NotifyEndApplication();

    /// <inheritdoc/>
    public override void SetShouldExit(int exitCode)
        => _host.SetShouldExit(exitCode);
}
