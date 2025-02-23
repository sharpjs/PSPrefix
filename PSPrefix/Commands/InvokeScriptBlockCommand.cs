// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using Microsoft.PowerShell.Commands;

namespace PSPrefix.Commands;

using InputStream  = PSDataCollection<PSObject?>;
using OutputStream = PSDataCollection<PSObject?>;
using ErrorStream  = PSDataCollection<ErrorRecord>;

/// <summary>
///   Base class for a command that invokes a script block in a new runspace.
/// </summary>
public abstract class InvokeScriptBlockCommand : PSCmdlet
{
    private const InputStream? NoInput = null;

    private ScriptBlock? _scriptBlock;
    private object[]?    _modules;
    private object[]?    _variables;

    /// <summary>
    ///   <b>-ScriptBlock:</b>
    ///   Script block to execute.
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
    ///   Modules to import before invoking <see cref="ScriptBlock"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Specify a module by its name or by a <see cref="PSModuleInfo"/>
    ///       object, such as one returned by the <c>Get-Module</c> command.
    ///   </para>
    ///   <para>
    ///     This command imports modules in a child runspace used to execute
    ///     the <c>-ScriptBlock</c>.  This command does not import the modules
    ///     into the containing runspace.
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
    ///   Variables to predefine before invoking <see cref="ScriptBlock"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Specify a variable by its name or by a <see cref="PSVariable"/>
    ///       object, such as one returned by the <c>Get-Variable</c> command.
    ///   </para>
    ///   <para>
    ///     TODO: Child runspace.
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
    ///   Use a custom PSHost.
    /// </summary>
    [Parameter(ValueFromPipelineByPropertyName = true)]
    [ValidateNotNull]
    public PSHost? CustomHost { get; set; }

    /// <summary>
    ///   <b>-PassThru:</b>
    ///   Write script block output objects to the pipeline.  If this switch is
    ///   not present, the command sends script block output objects to the
    ///   <c>Out-Default</c> command, which formats objects and displays them
    ///   on the current host.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    /// <summary>
    ///   Gets the PowerShell host to use when invoking the script block, or
    ///   <see langword="null"/> to use PowerShell's default host.
    /// </summary>
    protected internal virtual PSHost EffectiveHost => CustomHost ?? Host;

#if PRESERVE_PREFERENCE_VARIABLES_MAYBE
    protected virtual IEnumerable<object> AutoVariables => [
        "ConfirmPreference",
        "DebugPreference",
        "ErrorActionPreference",
        "InformationPreference",
        "ProgressPreference",
        "PSNativeCommandUseErrorActionPreference",
        "VerbosePreference",
        "WarningPreference",
        "WhatIfPreference",
    ];
#endif

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

        powershell.Invoke(NoInput, output, settings: new());
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
