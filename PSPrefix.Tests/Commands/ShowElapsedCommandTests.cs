// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Commands;

using static ConsoleColor;

[TestFixture]
public class ShowElapsedCommandTests : CommandTests
{
    public ShowElapsedCommandTests()
    {
        RawUI.SetupProperty(u => u.ForegroundColor);
        RawUI.SetupProperty(u => u.BackgroundColor);

        RawUI.Object.ForegroundColor = White;
        RawUI.Object.BackgroundColor = Black;
    }

    [Test]
    public void EffectiveHost_Initial()
    {
        new ShowElapsedCommand().EffectiveHost.ShouldBeNull();
    }

    [Test]
    public void Invoke_Empty()
    {
        Execute("Show-Elapsed { }");
    }

    [Test]
    public void Invoke_UIWrite()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.Write("a")).Verifiable();

        Execute("Show-Elapsed { $Host.UI.Write('a') }");
    }

    [Test]
    public void Invoke_UIWriteLine0()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.WriteLine()).Verifiable();

        Execute("Show-Elapsed { $Host.UI.WriteLine() }");
    }

    [Test]
    public void Invoke_UIWriteLine1()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.WriteLine("a")).Verifiable();

        Execute("Show-Elapsed { $Host.UI.WriteLine('a') }");
    }

    [Test]
    public void Invoke_WriteHost0()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.WriteLine(White, Black, "")).Verifiable();

        Execute("Show-Elapsed { Write-Host }");
    }

    [Test]
    public void Invoke_WriteHost1()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.WriteLine(White, Black, "a")).Verifiable();

        Execute("Show-Elapsed { Write-Host a }");
    }

    [Test]
    public void Invoke_WriteHost1_NoNewline()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.Write(White, Black, "a")).Verifiable();

        Execute("Show-Elapsed { Write-Host a -NoNewline }");
    }

    [Test]
    public void Invoke_WriteError()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.WriteErrorLine("ERROR: a")).Verifiable();

        Execute("Show-Elapsed { Write-Error a -ErrorAction Continue }");
    }

    [Test]
    public void Invoke_WriteWarning()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.WriteWarningLine("a")).Verifiable();

        Execute("Show-Elapsed { Write-Warning a }");
    }

    [Test]
    public void Invoke_WriteInformation()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.WriteLine("a")).Verifiable();

        Execute("Show-Elapsed { $InformationPreference = 'Continue'; Write-Information a }");
    }

    [Test]
    public void Invoke_WriteVerbose()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.WriteVerboseLine("a")).Verifiable();

        Execute("Show-Elapsed { $VerbosePreference = 'Continue'; Write-Verbose a }");
    }

    [Test]
    public void Invoke_WriteDebug()
    {
        var s = new MockSequence();
        ExpectWriteElapsed(s);
        UI.InSequence(s).Setup(u => u.WriteDebugLine("a")).Verifiable();

        Execute("Show-Elapsed { $DebugPreference = 'Continue'; Write-Debug a }");
    }

    [Test]
    public void Invoke_Module()
    {
        var obj = ExecuteSingle(
            "Show-Prefixed Foo { Get-Module PSReadLine } -Module (Get-Module PSPrefix), PSReadLine -PassThru"
        );

        obj.ShouldBeOfType<PSModuleInfo>();
    }

    [Test]
    public void Invoke_Variable()
    {
        var obj = ExecuteSingle(
            "$x = 42; Show-Prefixed Foo { $x + 1 } -Variable x -PassThru"
        );

        obj.ShouldBe(43);
    }

    private const string
        ElapsedPattern =
        """
        (?x) \[ \+ [0-9]{2} : [0-9]{2} : [0-9]{2} \] [ ]
        """;

    private void ExpectWriteElapsed(MockSequence s)
    {
        UI.InSequence(s)
            .Setup(u => u.Write(DarkGray, Black, It.IsRegex(ElapsedPattern)))
            .Verifiable();
    }

    private void Execute(string script)
    {
        var (output, exception) = ScriptExecutor.Execute(Host.Object, script);

        output   .ShouldBeEmpty();
        exception.ShouldBeNull();
    }

    private object ExecuteSingle(string script)
    {
        var (output, exception) = ScriptExecutor.Execute(Host.Object, script);

        output   .ShouldHaveSingleItem().ShouldNotBeNull().AssignTo(out var obj);
        exception.ShouldBeNull();

        return obj.BaseObject;
    }
}
