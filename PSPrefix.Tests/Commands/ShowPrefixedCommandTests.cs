// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Commands;

using static ConsoleColor;

[TestFixture]
public class ShowPrefixedCommandTests : CommandTests
{
    public ShowPrefixedCommandTests()
    {
        RawUI.SetupProperty(u => u.ForegroundColor);
        RawUI.SetupProperty(u => u.BackgroundColor);

        RawUI.Object.ForegroundColor = White;
        RawUI.Object.BackgroundColor = Black;
    }

    [Test]
    public void EffectiveHost_Initial()
    {
        new ShowPrefixedCommand().EffectiveHost.ShouldBeNull();
    }

    [Test]
    public void Invoke_Empty()
    {
        Execute("Show-Prefixed Foo { }");
    } 
    [Test]
    public void Invoke_UIWrite()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write(DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.Write(                 "a"     )).Verifiable();

        Execute("Show-Prefixed Foo { $Host.UI.Write('a') }");
    }

    [Test]
    public void Invoke_UIWriteLine0()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write    (DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteLine(                         )).Verifiable();

        Execute("Show-Prefixed Foo { $Host.UI.WriteLine() }");
    }

    [Test]
    public void Invoke_UIWriteLine1()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write    (DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteLine(                 "a"     )).Verifiable();

        Execute("Show-Prefixed Foo { $Host.UI.WriteLine('a') }");
    }

    [Test]
    public void Invoke_WriteHost0()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write    (DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteLine(White,    Black, ""      )).Verifiable();

        Execute("Show-Prefixed Foo { Write-Host }");
    }

    [Test]
    public void Invoke_WriteHost1()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write    (DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteLine(White,    Black, "a"     )).Verifiable();

        Execute("Show-Prefixed Foo { Write-Host a }");
    }

    [Test]
    public void Invoke_WriteHost1_NoNewline()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write(DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.Write(White,    Black, "a"     )).Verifiable();

        Execute("Show-Prefixed Foo { Write-Host a -NoNewline }");
    }

    [Test]
    public void Invoke_WriteOutput()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write    (DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteLine(                 "a"     )).Verifiable();

        Execute("Show-Prefixed Foo { Write-Output a }");
    }

    [Test]
    public void Invoke_WriteOutput_PassThru()
    {
        ExecuteYields<string>(
            "Show-Prefixed Foo { Write-Output a } -PassThru",
            x => x.ShouldBe("a")
        );
    }

    [Test]
    public void Invoke_WriteError()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write         (DarkBlue, Black, "[Foo] "  )).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteErrorLine(                 "ERROR: a")).Verifiable();

        Execute("Show-Prefixed Foo { Write-Error a }");
    }

    [Test]
    public void Invoke_WriteWarning()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write           (DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteWarningLine(                 "a"     )).Verifiable();

        Execute("Show-Prefixed Foo { Write-Warning a }");
    }

    [Test]
    public void Invoke_WriteInformation()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write    (DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteLine(                 "a"     )).Verifiable();

        Execute("Show-Prefixed Foo { $InformationPreference = 'Continue'; Write-Information a }");
    }

    [Test]
    public void Invoke_WriteVerbose()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write           (DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteVerboseLine(                 "a"     )).Verifiable();

        Execute("Show-Prefixed Foo { $VerbosePreference = 'Continue'; Write-Verbose a }");
    }

    [Test]
    public void Invoke_WriteDebug()
    {
        var s = new MockSequence();
        UI.InSequence(s).Setup(u => u.Write         (DarkBlue, Black, "[Foo] ")).Verifiable();
        UI.InSequence(s).Setup(u => u.WriteDebugLine(                 "a"     )).Verifiable();

        Execute("Show-Prefixed Foo { $DebugPreference = 'Continue'; Write-Debug a }");
    }

    [Test]
    public void Invoke_Module()
    {
        ExecuteYields<PSModuleInfo>(
            """
            Show-Prefixed Foo { Get-Module PSPrefix, PSReadLine } -PassThru `
                -Module (Get-Module PSPrefix), PSReadLine
            """,
            x => x.Name.ShouldBe("PSPrefix"),
            x => x.Name.ShouldBe("PSReadLine")
        );
    }

    [Test]
    public void Invoke_Variable()
    {
        ExecuteYields<int>(
            """
            $x = 42
            $y =  3
            Show-Prefixed Foo { $x + $y } -PassThru `
                -Variable (Get-Variable x), y
            """,
            x => x.ShouldBe(45)
        );
    }

    private void Execute(string script)
    {
        var (output, exception) = ScriptExecutor.Execute(Host.Object, script);

        exception.ShouldBeNull();
        output   .ShouldBeEmpty();
    }

    private void ExecuteYields<T>(string script, params Action<T>[] predicates)
    {
        var (output, exception) = ScriptExecutor.Execute(Host.Object, script);

        exception   .ShouldBeNull();
        output.Count.ShouldBe(predicates.Length);

        for (var i = 0; i < predicates.Length; i++)
            predicates[i](output[i].ShouldNotBeNull().BaseObject.ShouldBeOfType<T>());
    }

    private void ExecuteThrows<T>(string script)
        where T : Exception
    {
        var (output, exception) = ScriptExecutor.Execute(Host.Object, script);

        output   .ShouldBeEmpty();
        exception.ShouldBeOfType<T>();
    }
}
