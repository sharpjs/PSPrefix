// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Collections.ObjectModel;
using System.Security;

namespace PSPrefix.Internal;

/// <summary>
///   A PowerShell host user interface wrapper that prepends a prefix to output
///   lines.
/// </summary>
internal class PrefixedHostUI : PSHostUserInterface
{
    const char
        Cr = '\r',
        Lf = '\n';

    private readonly PSHostUserInterface _ui;
    private readonly PrefixedHostRawUI   _rawUI;
    private readonly IPrefix             _prefix;

    private bool _isAtBol;

    /// <summary>
    ///   Initializes a new <see cref="PrefixedHostUI"/> instance.
    /// </summary>
    /// <param name="ui">
    ///   The underlying host user interface.
    /// </param>
    /// <param name="prefix">
    ///   The prefix.
    /// </param>
    internal PrefixedHostUI(PSHostUserInterface ui, IPrefix prefix)
    {
        _ui      = ui;
        _rawUI   = new PrefixedHostRawUI(_ui.RawUI, prefix);
        _prefix  = prefix;
        _isAtBol = true;
    }

    /// <inheritdoc/>
    public override PSHostRawUserInterface RawUI
        => _rawUI;

    /// <inheritdoc/>
    public override bool SupportsVirtualTerminal
        => _ui.SupportsVirtualTerminal;

    /// <inheritdoc/>
    public override void Write(string value)
    {
        var start = 0;

        do
        {
            Prepare();
            var (eol, limit) = FindBol(value, start);
            _ui.Write(value[start..limit]);
            Update(eol);
            start = limit;
        }
        while (start < value.Length);
    }

    /// <inheritdoc/>
    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        var start = 0;

        do
        {
            Prepare();
            var (eol, limit) = FindBol(value, start);
            _ui.Write(foregroundColor, backgroundColor, value[start..limit]);
            Update(eol);
            start = limit;
        }
        while (start < value.Length);
    }

    /// <inheritdoc/>
    public override void WriteLine()
    {
        Prepare();
        _ui.WriteLine();
        Update(eol: true);
    }

    /// <inheritdoc/>
    public override void WriteLine(string value)
    {
        var start = 0;

        for (;;)
        {
            Prepare();
            var (more, limit, next) = FindEol(value, start);
            _ui.WriteLine(value[start..limit]);
            Update(eol: true);
            if (!more) return;
            start = next;
        }
    }

    /// <inheritdoc/>
    public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        for (var start = 0;;)
        {
            Prepare();
            var (more, limit, next) = FindEol(value, start);
            _ui.WriteLine(foregroundColor, backgroundColor, value[start..limit]);
            Update(eol: true);
            if (!more) return;
            start = next;
        }
    }

    /// <inheritdoc/>
    public override void WriteDebugLine(string message)
    {
        for (var start = 0;;)
        {
            Prepare();
            var (more, limit, next) = FindEol(message, start);
            _ui.WriteDebugLine(message[start..limit]);
            Update(eol: true);
            if (!more) return;
            start = next;
        }
    }

    /// <inheritdoc/>
    public override void WriteVerboseLine(string message)
    {
        for (var start = 0;;)
        {
            Prepare();
            var (more, limit, next) = FindEol(message, start);
            _ui.WriteVerboseLine(message[start..limit]);
            Update(eol: true);
            if (!more) return;
            start = next;
        }
    }

    /// <inheritdoc/>
    public override void WriteWarningLine(string message)
    {
        for (var start = 0;;)
        {
            Prepare();
            var (more, limit, next) = FindEol(message, start);
            _ui.WriteWarningLine(message[start..limit]);
            Update(eol: true);
            if (!more) return;
            start = next;
        }
    }

    /// <inheritdoc/>
    public override void WriteErrorLine(string value)
    {
        // Match ConsoleHostUserInterface special-case behavior
        if (string.IsNullOrEmpty(value))
            return;

        for (var start = 0;;)
        {
            Prepare();
            var (more, limit, next) = FindEol(value, start);
            WriteErrorLineCore(value[start..limit]);
            Update(eol: true);
            if (!more) return;
            start = next;
        }
    }

    private void WriteErrorLineCore(string message)
    {
        var previousColor = _rawUI.ForegroundColor;
        try
        {
            _rawUI.ForegroundColor = ConsoleColor.Red;
            _ui.WriteErrorLine("ERROR: " + message);
        }
        finally
        {
            _rawUI.ForegroundColor = previousColor;
        }
    }

    /// <inheritdoc/>
    public override void WriteInformation(InformationRecord record)
    {
        // NOTE: Do not modify record; doing so results in duplicate prefixes
        _ui.WriteInformation(record);
    }

    /// <inheritdoc/>
    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        // NOTE: Do not modify record; it is not presented in the textual log
        _ui.WriteProgress(sourceId, record);
    }

    /// <inheritdoc/>
    public override string ReadLine()
    {
        var result = _ui.ReadLine();
        Update(eol: true);
        return result;
    }

    /// <inheritdoc/>
    public override SecureString ReadLineAsSecureString()
    {
        var result = _ui.ReadLineAsSecureString();
        Update(eol: true);
        return result;
    }

    /// <inheritdoc/>
    public override Dictionary<string, PSObject> Prompt(
        string caption, string message, Collection<FieldDescription> descriptions)
    {
        return _ui.Prompt(caption, message, descriptions);
    }

    /// <inheritdoc/>
    public override int PromptForChoice(
        string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
    {
        return _ui.PromptForChoice(caption, message, choices, defaultChoice);
    }

    /// <inheritdoc/>
    public override PSCredential PromptForCredential(
        string caption, string message, string userName, string targetName)
    {
        return _ui.PromptForCredential(caption, message, userName, targetName);
    }

    /// <inheritdoc/>
    public override PSCredential PromptForCredential(
        string caption, string message, string userName, string targetName,
        PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
    {
        return _ui.PromptForCredential(
            caption, message, userName, targetName, allowedCredentialTypes, options
        );
    }

    private void Prepare()
    {
        if (_isAtBol)
            _prefix.Write(_ui);
    }

    private void Update(bool eol)
    {
        _isAtBol = eol;
    }

    private static (bool, int) FindBol(string text, int start)
    {
        var index = text.IndexOf(Lf, start);

        return index < 0
            ? (false, text.Length)
            : (true,  index + 1);
    }

    private static (bool, int, int) FindEol(string text, int start)
    {
        var lfIndex = text.IndexOf(Lf, start);

        return lfIndex < 0
            ? (false, text.Length,                 text.Length)
            : (true,  RewindIfCrLf(text, lfIndex), lfIndex + 1);
    }

    private static int RewindIfCrLf(string text, int lfIndex)
    {
        if (lfIndex is 0)
            return lfIndex;

        var crIndex = lfIndex - 1;

        return text[crIndex] is Cr ? crIndex : lfIndex;
    }
}
