// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Collections.ObjectModel;
using System.Security;

namespace PSPrefix.Internal;

[TestFixture]
public class SynchronizedHostUITests : PSHostUITests
{
    private new SynchronizedHostUI UI => (SynchronizedHostUI) base.UI;

    [Test]
    public void Lock_Get()
    {
        UI.Lock.ShouldNotBeNull();
    }

    [Test]
    public void RawUI_Get()
    {
        UI.RawUI.ShouldBeOfType<SynchronizedHostRawUI>();
    }

    [Test]
    public void SupportsVirtualTerminal_Get([Values(false, true)] bool value)
    {
        Expect(u => u.SupportsVirtualTerminal, result: value);

        UI.SupportsVirtualTerminal.ShouldBe(value);
    }

    [Test]
    public void Write1()
    {
        InnerUI
            .Setup(u => u.Write("a"))
            .Verifiable();

        UI.Write("a");
    }

    [Test]
    public void Write3()
    {
        InnerUI
            .Setup(u => u.Write(ConsoleColor.Yellow, ConsoleColor.DarkRed, "a"))
            .Verifiable();

        UI.Write(ConsoleColor.Yellow, ConsoleColor.DarkRed, "a");
    }

    [Test]
    public void WriteLine0()
    {
        InnerUI
            .Setup(u => u.WriteLine())
            .Verifiable();

        UI.WriteLine();
    }

    [Test]
    public void WriteLine1()
    {
        InnerUI
            .Setup(u => u.WriteLine("a"))
            .Verifiable();

        UI.WriteLine("a");
    }

    [Test]
    public void WriteLine3()
    {
        InnerUI
            .Setup(u => u.WriteLine(ConsoleColor.Yellow, ConsoleColor.DarkRed, "a"))
            .Verifiable();

        UI.WriteLine(ConsoleColor.Yellow, ConsoleColor.DarkRed, "a");
    }

    [Test]
    public void WriteErrorLine()
    {
        InnerUI
            .Setup(u => u.WriteErrorLine("a"))
            .Verifiable();

        UI.WriteErrorLine("a");
    }

    [Test]
    public void WriteWarningLine()
    {
        InnerUI
            .Setup(u => u.WriteWarningLine("a"))
            .Verifiable();

        UI.WriteWarningLine("a");
    }

    [Test]
    public void WriteVerboseLine()
    {
        InnerUI
            .Setup(u => u.WriteVerboseLine("a"))
            .Verifiable();

        UI.WriteVerboseLine("a");
    }

    [Test]
    public void WriteDebugLine()
    {
        InnerUI
            .Setup(u => u.WriteDebugLine("a"))
            .Verifiable();

        UI.WriteDebugLine("a");
    }

    [Test]
    public void WriteInformation()
    {
        var record = new InformationRecord("a", "b");

        InnerUI
            .Setup(u => u.WriteInformation(record))
            .Verifiable();

        UI.WriteInformation(record);
    }

    [Test]
    public void WriteProgress()
    {
        var record = new ProgressRecord(1337, "a", "b");

        InnerUI
            .Setup(u => u.WriteProgress(42L, record))
            .Verifiable();

        UI.WriteProgress(42L, record);
    }

    [Test]
    public void ReadLine()
    {
        InnerUI
            .Setup(u => u.ReadLine())
            .Returns("a")
            .Verifiable();

        UI.ReadLine().ShouldBe("a");
    }

    [Test]
    public void ReadLineAsSecureString()
    {
        using var s = new SecureString();

        s.AppendChar('a');
        s.MakeReadOnly();

        InnerUI
            .Setup(u => u.ReadLineAsSecureString())
            .Returns(s)
            .Verifiable();

        UI.ReadLineAsSecureString().ShouldBeSameAs(s);
    }

    [Test]
    public void Prompt()
    {
        var descriptions = new Collection<FieldDescription>();
        var result       = new Dictionary<string, PSObject>();

        InnerUI
            .Setup(u => u.Prompt("a", "b", descriptions))
            .Returns(result)
            .Verifiable();

        UI.Prompt("a", "b", descriptions).ShouldBeSameAs(result);
    }

    [Test]
    public void PromptForChoice()
    {
        var descriptions = new Collection<ChoiceDescription>();
        var result       = new Dictionary<string, PSObject>();

        InnerUI
            .Setup(u => u.PromptForChoice("a", "b", descriptions, 3))
            .Returns(2)
            .Verifiable();

        UI.PromptForChoice("a", "b", descriptions, 3).ShouldBe(2);
    }

    [Test]
    public void PromptForCredential4()
    {
        using var password = new SecureString();

        password.AppendChar('p');
        password.MakeReadOnly();

        var credential = new PSCredential("u", password);

        InnerUI
            .Setup(u => u.PromptForCredential("a", "b", "c", "d"))
            .Returns(credential)
            .Verifiable();

        UI.PromptForCredential("a", "b", "c", "d").ShouldBeSameAs(credential);
    }

    [Test]
    public void PromptForCredential6()
    {
        using var password = new SecureString();

        password.AppendChar('p');
        password.MakeReadOnly();

        var credential = new PSCredential("u", password);
        var types      = PSCredentialTypes.Generic;
        var options    = PSCredentialUIOptions.AlwaysPrompt;

        InnerUI
            .Setup(u => u.PromptForCredential("a", "b", "c", "d", types, options))
            .Returns(credential)
            .Verifiable();

        UI.PromptForCredential("a", "b", "c", "d", types, options).ShouldBeSameAs(credential);
    }

    protected override PSHostUserInterface CreateHostUI(PSHostUserInterface ui)
        => new SynchronizedHostUI(ui, new());
}
