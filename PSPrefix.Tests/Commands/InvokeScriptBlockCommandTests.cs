// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Reflection;

namespace PSPrefix.Commands;

[TestFixture]
public class InvokeScriptBlockCommandTests
{
    private static readonly ConstructorInfo PSModuleInfoConstructor;

    static InvokeScriptBlockCommandTests()
    {
        // https://github.com/PowerShell/PowerShell/blob/v7.4.7/src/System.Management.Automation/engine/Modules/PSModuleInfo.cs#L77
        // internal PSModuleInfo(string, string, SMA.ExecutionContext, SMA.SessionState)

        var moduleInfoType = typeof(PSModuleInfo);

        var executionContextType = moduleInfoType
            .Assembly
            .GetType("System.Management.Automation.ExecutionContext")
            .ShouldNotBeNull();

        PSModuleInfoConstructor = moduleInfoType
            .GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                [typeof(string), typeof(string), executionContextType, typeof(SessionState)]
            )
            .ShouldNotBeNull();
    }

    [Test]
    public void ScriptBlock_Initial()
    {
        new TestCommand().ScriptBlock.ToString().ShouldBeEmpty();
    }

    [Test]
    public void Module_Initial()
    {
        new TestCommand().Module.ShouldBeEmpty();
    }

    [Test]
    public void Variable_Initial()
    {
        new TestCommand().Variable.ShouldBeEmpty();
    }

    [Test]
    public void CustomHost_Initial()
    {
        new TestCommand().CustomHost.ShouldBeNull();
    }

    [Test]
    public void GetModules_Empty()
    {
        new TestCommand().GetModules().ShouldBeEmpty();
    }

    [Test]
    public void GetModules_NullItem()
    {
        new TestCommand { Module = [null!] }.GetModules().ShouldBeEmpty();
    }

    [Test]
    public void GetModules_Invalid()
    {
        Should.Throw<InvalidOperationException>(() =>
        {
            new TestCommand { Module = [42] }.GetModules().Count();
        });
    }

    [Test]
    public void GetModules_Invalid_InPSObject()
    {
        Should.Throw<InvalidOperationException>(() =>
        {
            new TestCommand { Module = [new PSObject(42)] }.GetModules().Count();
        });
    }

    [Test]
    public void GetModules_String()
    {
        new TestCommand { Module = ["A", "B"] }
            .GetModules().Select(x => x.Name).ShouldBe(["A", "B"]);
    }

    [Test]
    public void GetModules_String_InPSObject()
    {
        new TestCommand { Module = [new PSObject("A"), new PSObject("B")] }
            .GetModules().Select(x => x.Name).ShouldBe(["A", "B"]);
    }

    [Test]
    public void GetModules_String_Duplicate()
    {
        new TestCommand { Module = ["A", "A"] }
            .GetModules().Select(x => x.Name).ShouldBe(["A"]);
    }

    [Test]
    public void GetModules_PSModuleInfo()
    {
        var a = MakeModuleInfo("A");
        var b = MakeModuleInfo("B");

        new TestCommand { Module = [a, b] }
            .GetModules().Select(x => x.Name).ShouldBe([a.Path, b.Path]);
    }

    [Test]
    public void GetModules_PSModuleInfo_InPSObject()
    {
        var a = MakeModuleInfo("A");
        var b = MakeModuleInfo("B");

        new TestCommand { Module = [new PSObject(a), new PSObject(b)] }
            .GetModules().Select(x => x.Name).ShouldBe([a.Path, b.Path]);
    }

    [Test]
    public void GetModules_PSModuleInfo_Duplicate()
    {
        var a0 = MakeModuleInfo("A",  MakePath("A"));
        var a1 = MakeModuleInfo("A",  MakePath("AX")); // dupe name
        var a2 = MakeModuleInfo("AX", MakePath("A"));  // dupe path

        new TestCommand { Module = [a0, a1, a2] }
            .GetModules().Select(x => x.Name).ShouldBe([a0.Path]);
    }

    [Test]
    public void GetVariables_Empty()
    {
        new TestCommand().GetVariables().ShouldBeEmpty();
    }

    [Test]
    public void GetVariables_NullItem()
    {
        new TestCommand { Variable = [null!] }.GetVariables().ShouldBeEmpty();
    }

    [Test]
    public void GetVariables_Invalid()
    {
        Should.Throw<InvalidOperationException>(() =>
        {
            new TestCommand { Variable = [42] }.GetVariables().Count();
        });
    }

    [Test]
    public void GetVariables_Invalid_InPSObject()
    {
        Should.Throw<InvalidOperationException>(() =>
        {
            new TestCommand { Variable = [new PSObject(42)] }.GetVariables().Count();
        });
    }

    [Test]
    public void GetVariables_String()
    {
        var a = MakeVariable("A");
        var b = MakeVariable("B");

        new TestCommand(a, b) { Variable = ["A", "B"] }
            .GetVariables().Select(x => x.Name).ShouldBe(["A", "B"]);
    }

    [Test]
    public void GetVariables_String_InPSObject()
    {
        var a = MakeVariable("A");
        var b = MakeVariable("B");

        new TestCommand(a, b) { Variable = [new PSObject("A"), new PSObject("B")] }
            .GetVariables().Select(x => x.Name).ShouldBe(["A", "B"]);
    }

    [Test]
    public void GetVariables_String_Duplicate()
    {
        var a = MakeVariable("A");

        new TestCommand(a) { Variable = ["A", "A"] }
            .GetVariables().Select(x => x.Name).ShouldBe(["A"]);
    }

    [Test]
    public void GetVariables_String_NotFound()
    {
        new TestCommand { Variable = ["A"] }
            .GetVariables().Should().BeEmpty();
    }

    [Test]
    public void GetVariables_PSVariable()
    {
        var a = MakeVariable("A");
        var b = MakeVariable("B");

        new TestCommand { Variable = [a, b] }
            .GetVariables().Select(x => x.Name).ShouldBe([a.Name, b.Name]);
    }

    [Test]
    public void GetVariables_PSVariable_InPSObject()
    {
        var a = MakeVariable("A");
        var b = MakeVariable("B");

        new TestCommand { Variable = [new PSObject(a), new PSObject(b)] }
            .GetVariables().Select(x => x.Name).ShouldBe([a.Name, b.Name]);
    }

    [Test]
    public void GetVariables_PSVariable_Duplicate()
    {
        var a0 = MakeVariable("A");
        var a1 = MakeVariable("A");

        new TestCommand { Variable = [a0, a1] }
            .GetVariables().Select(x => x.Name).ShouldBe([a0.Name]);
    }

    [Test]
    public void FlowErrorStream_NullUI()
    {
        using var shell = PowerShell.Create();

        new TestCommand().FlowErrorStream(shell);

        GetDataAddedEventHandler(shell.Streams.Error).Should().BeNull();
    }

    private static string MakePath(string name)
    {
        return Path.GetFullPath(name + ".psd1");
    }

    private static PSModuleInfo MakeModuleInfo(string name, string? path = null)
    {
        return PSModuleInfoConstructor
            .Invoke([name, path ?? MakePath(name), null, null])
            .ShouldBeOfType<PSModuleInfo>();
    }

    private static PSVariable MakeVariable(string name)
    {
        return new PSVariable(name);
    }

    private static Delegate? GetDataAddedEventHandler<T>(PSDataCollection<T> stream)
    {
        const BindingFlags Event
            = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        return (Delegate?) stream
            .GetType()
            .GetField(nameof(stream.DataAdded), Event).ShouldNotBeNull()
            .GetValue(stream);
    }

    private class TestCommand : InvokeScriptBlockCommand
    {
        private static StringComparer
            KeyComparer => StringComparer.OrdinalIgnoreCase;

        private static readonly Dictionary<string, PSVariable>
            Empty = new(KeyComparer);

        private readonly IReadOnlyDictionary<string, PSVariable> _variables;

        public TestCommand()
        {
            _variables = Empty;
        }

        public TestCommand(params PSVariable[] predefinedVariables)
        {
            _variables = predefinedVariables.ToDictionary(v => v.Name, KeyComparer);
        }

        internal override PSVariable? GetVariable(string name)
        {
            return _variables.TryGetValue(name, out var variable) ? variable : null;
        }
    }
}
