// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Collections.ObjectModel;
using System.Security;

namespace PSPrefix.Internal;

/// <summary>
///   A PowerShell host user interface wrapper that is safe to share across
///   threads.
/// </summary>
/// <remarks>
///   All members of this class are thread-safe.
/// </remarks>
public class SynchronizedHostUI : PSHostUserInterface
{
    private readonly PSHostUserInterface   _ui;
    private readonly SynchronizedHostRawUI _rawUI;
    private readonly object                _lock;

    /// <summary>
    ///   Initializes a new <see cref="SynchronizedHostUI"/> instance.
    /// </summary>
    /// <param name="ui">
    ///   The underlying host raw user interface.
    /// </param>
    /// <param name="lock">
    ///   The lock object.
    /// </param>
    internal SynchronizedHostUI(PSHostUserInterface ui, object @lock)
    {
        _ui    = ui;
        _rawUI = new(ui.RawUI, @lock);
        _lock  = @lock;
    }

    /// <inheritdoc/>
    public override PSHostRawUserInterface RawUI
    {
        get { lock (_lock) return _rawUI; }
    }

    /// <inheritdoc/>
    public override bool SupportsVirtualTerminal
    {
        get { lock (_lock) return _ui.SupportsVirtualTerminal; }
    }

    /// <inheritdoc/>
    public override void Write(string text)
    {
        lock (_lock) _ui.Write(text);
    }

    /// <inheritdoc/>
    public override void Write(ConsoleColor foreground, ConsoleColor background, string text)
    {
        lock (_lock) _ui.Write(foreground, background, text);
    }

    /// <inheritdoc/>
    public override void WriteLine()
    {
        lock (_lock) _ui.WriteLine();
    }

    /// <inheritdoc/>
    public override void WriteLine(string text)
    {
        lock (_lock) _ui.WriteLine(text);
    }

    /// <inheritdoc/>
    public override void WriteLine(ConsoleColor foreground, ConsoleColor background, string value)
    {
        lock (_lock) _ui.WriteLine(foreground, background, value);
    }

    /// <inheritdoc/>
    public override void WriteDebugLine(string text)
    {
        lock (_lock) _ui.WriteDebugLine(text);
    }

    /// <inheritdoc/>
    public override void WriteVerboseLine(string text)
    {
        lock (_lock) _ui.WriteVerboseLine(text);
    }

    /// <inheritdoc/>
    public override void WriteWarningLine(string text)
    {
        lock (_lock) _ui.WriteWarningLine(text);
    }

    /// <inheritdoc/>
    public override void WriteErrorLine(string text)
    {
        lock (_lock) _ui.WriteErrorLine(text);
    }

    /// <inheritdoc/>
    public override void WriteInformation(InformationRecord record)
    {
        lock (_lock) _ui.WriteInformation(record);
    }

    /// <inheritdoc/>
    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        lock (_lock) _ui.WriteProgress(sourceId, record);
    }

    /// <inheritdoc/>
    public override string ReadLine()
    {
        lock (_lock) return _ui.ReadLine();
    }

    /// <inheritdoc/>
    public override SecureString ReadLineAsSecureString()
    {
        lock (_lock) return _ui.ReadLineAsSecureString();
    }

    /// <inheritdoc/>
    public override Dictionary<string, PSObject> Prompt(
        string caption, string message, Collection<FieldDescription> descriptions)
    {
        lock (_lock) return _ui.Prompt(caption, message, descriptions);
    }

    /// <inheritdoc/>
    public override int PromptForChoice(
        string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
    {
        lock (_lock) return _ui.PromptForChoice(caption, message, choices, defaultChoice);
    }

    /// <inheritdoc/>
    public override PSCredential PromptForCredential(
        string caption, string message, string userName, string targetName)
    {
        lock (_lock) return _ui.PromptForCredential(caption, message, userName, targetName);
    }

    /// <inheritdoc/>
    public override PSCredential PromptForCredential(
        string caption, string message, string userName, string targetName,
        PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
    {
        lock (_lock) return _ui.PromptForCredential(
            caption, message, userName, targetName, allowedCredentialTypes, options
        );
    }
}
