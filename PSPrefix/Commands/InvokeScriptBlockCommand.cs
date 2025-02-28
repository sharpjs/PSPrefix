// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using Microsoft.PowerShell.Commands;

namespace PSPrefix.Commands;

using InputStream  = PSDataCollection<PSObject?>;
using OutputStream = PSDataCollection<PSObject?>;
using ErrorStream  = PSDataCollection<ErrorRecord>;

/// <summary>
///   Base class for a cmdlet that invokes a script block in a new runspace.
/// </summary>
public abstract class InvokeScriptBlockCommand : PSCmdlet
{
    private const InputStream? NoInput = null;

    private ScriptBlock? _scriptBlock;
    private object[]?    _modules;
    private object[]?    _variables;

    /// <summary>
    ///   <b>-ScriptBlock:</b>
    ///   The script block to invoke.
    /// </summary>
    [Parameter(
        Mandatory                       = true,
        Position                        = 1,
        ValueFromPipeline               = true,
        ValueFromPipelineByPropertyName = true
    )]
    [ValidateNotNull]
    public ScriptBlock ScriptBlock
    {
        get => _scriptBlock ??= ScriptBlock.Create("");
        set => _scriptBlock   = value;
    }

    /// <summary>
    ///   <b>-Module:</b>
    ///   Modules to import before invoking the <see cref="ScriptBlock"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Specify a module by its name or by a <see cref="PSModuleInfo"/>
    ///       object, such as one returned by the <c>Get-Module</c> cmdlet.
    ///   </para>
    ///   <para>
    ///     Modules imported via this parameter are visible within the
    ///     <c>-ScriptBlock</c> only and do not affect the invoking runspace.
    ///   </para>
    /// </remarks>
    [Parameter(ValueFromPipelineByPropertyName = true)]
    [ValidateNotNull]
    [AllowEmptyCollection]
    public object[] Module
    {
        get => _modules ??= [];
        set => _modules   = value;
    }

    /// <summary>
    ///   <b>-Variable:</b>
    ///   Variables to define before invoking the <see cref="ScriptBlock"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Specify a variable by its name or by a <see cref="PSVariable"/>
    ///       object, such as one returned by the <c>Get-Variable</c> cmdlet.
    ///   </para>
    ///   <para>
    ///     Variables defined via this parameter are visible within the
    ///     <c>-ScriptBlock</c> only and do not affect the invoking runspace.
    ///   </para>
    ///   <para>
    ///     ⚠ <b>NOTE:</b> Variables defined via this parameter are shallow
    ///     copies that contain the same value object.  If that value object is
    ///     internally mutable and the <see cref="ScriptBlock"/> mutates it,
    ///     the change is visible to the invoking runspace.
    ///   </para>
    /// </remarks>
    [Parameter(ValueFromPipelineByPropertyName = true)]
    [ValidateNotNull]
    [AllowEmptyCollection]
    public object[] Variable
    {
        get => _variables ??= [];
        set => _variables   = value;
    }

    /// <summary>
    ///   <b>-CustomHost:</b>
    ///   A custom <c>PSHost</c> to which to send prefixed output lines.
    /// </summary>
    [Parameter(ValueFromPipelineByPropertyName = true)]
    [ValidateNotNull]
    public PSHost? CustomHost { get; set; }

    /// <summary>
    ///   <b>-PassThru:</b>
    ///   Forward the <see cref="ScriptBlock"/> Success stream output objects
    ///   to the invoking pipeline.  If this switch is not present, the cmdlet
    ///   sends the script block's Success stream to the <c>Out-Default</c>
    ///   cmdlet, which formats and displays the objects on the host.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    /// <summary>
    ///   Gets the PowerShell host to use when invoking the
    ///   <see cref="ScriptBlock"/>.
    /// </summary>
    protected internal virtual PSHost EffectiveHost => CustomHost ?? Host;

    // Lock to prevent race condition between PowerShell disposal and stop
    private readonly object _lock = new();

    // Delegate that stops the current invocation; non-null when stoppable
    private Action? _stopper;

    /// <inheritdoc/>
    protected override void ProcessRecord()
    {
        using var runspace = CreateRunspace();

        runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
        runspace.Open();

        using var powershell = PowerShell.Create(runspace);
              var output     = new OutputStream();

        FlowOutputStream(output);
        FlowErrorStream (powershell);

        powershell.AddScript(ScriptBlock.ToString());

        if (!PassThru)
            powershell.AddCommand("Out-Default");

        lock (_lock)
            _stopper = powershell.Stop;

        try
        {
            powershell.Invoke(NoInput, output, settings: new());
        }
        finally
        {
            lock (_lock)
                _stopper = null;
        }
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage] // Do not know how to trigger this during invocation other than Ctrl+C
    protected override void StopProcessing()
    {
        // This method runs on a different thread than that of ProcessRecord.
        // The lock ensures that if _stopper has a value, the corresponding
        // PowerShell instance has not been disposed yet.

        lock (_lock)
            _stopper?.Invoke();
    }

    private Runspace CreateRunspace()
    {
        var state = CreateInitialSessionState();

        return RunspaceFactory.CreateRunspace(EffectiveHost, state);
    }

    protected virtual InitialSessionState CreateInitialSessionState()
    {
        var state = InitialSessionState.CreateDefault();

        if (Module.Length > 0)
            state.ImportPSModule(GetModules());

        if (Variable.Length > 0)
            state.Variables.Add(GetVariables());

        return state;
    }

    internal IEnumerable<ModuleSpecification> GetModules()
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "" };

        foreach (var item in Module)
        {
            var candidate = item;

            if (candidate is PSObject wrapped)
                candidate = wrapped.BaseObject;

            if (candidate is null)
                continue;

            var (key, name) = candidate switch
            {
                PSModuleInfo i => (i.Name, i.Path),
                string       n => (n,      n     ),
                _              => throw OnInvalidModule(candidate),
            };

            if (!visited.Add(key))
                continue;

            if (!visited.Comparer.Equals(key, name) && !visited.Add(name))
                continue;

            yield return new(name);
        }
    }

    internal IEnumerable<SessionStateVariableEntry> GetVariables()
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "" };

        foreach (var item in Variable)
        {
            var candidate = item;

            if (candidate is PSObject wrapped)
                candidate = wrapped.BaseObject;

            if (candidate is null)
                continue;

            var variable = candidate switch
            {
                PSVariable v => v,
                string     n => GetVariable(n),
                _            => throw OnInvalidVariable(candidate),
            };

            if (variable is null)
                continue;

            if (!visited.Add(variable.Name))
                continue;

            yield return ToSessionStateVariableEntry(variable);
        }
    }

    // Test extensibility point
    internal virtual PSVariable? GetVariable(string name)
    {
        return SessionState.PSVariable.Get(name);
    }

    private static SessionStateVariableEntry ToSessionStateVariableEntry(PSVariable variable)
    {
        return new(
            variable.Name,
            variable.Value,
            variable.Description,
            variable.Options,
            variable.Attributes
        );
    }

    private static Exception OnInvalidModule(object candidate)
    {
        return new InvalidOperationException(string.Format(
            "Unable to interpret object of type '{0}' as a PowerShell module reference.",
            candidate.GetType().FullName
        ));
    }

    private static Exception OnInvalidVariable(object candidate)
    {
        return new InvalidOperationException(string.Format(
            "Unable to interpret object of type '{0}' as a PowerShell variable reference.",
            candidate.GetType().FullName
        ));
    }

    internal void FlowOutputStream(OutputStream output)
    {
        output.DataAdded += OnOutputAdded;

        void OnOutputAdded(object? sender, DataAddedEventArgs args)
        {
            var stream = (OutputStream) sender!;

            foreach (var obj in stream.ReadAll())
                WriteObject(obj);
        }
    }

    internal void FlowErrorStream(PowerShell powershell)
    {
        if (EffectiveHost is not { UI: { } ui })
            return;

        powershell.Streams.Error.DataAdded += OnErrorAdded;

        void OnErrorAdded(object? sender, DataAddedEventArgs args)
        {
            var stream = (ErrorStream) sender!;

            foreach (var error in stream.ReadAll())
                ui.WriteErrorLine(error.ToString());
        }
    }
}
