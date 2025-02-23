// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Security;

namespace PSPrefix.Internal;

using static ConsoleColor;

[TestFixture]
public class PrefixedHostUITests : PSHostUITests
{
    private Mock<IPrefix> Prefix { get; }

    private new PrefixedHostUI UI => (PrefixedHostUI) base.UI;

    public PrefixedHostUITests()
    {
        Prefix = Mocks.Create<IPrefix>();
    }

    [Test]
    public void RawUI_Get()
    {
        UI.RawUI.ShouldBeOfType<PrefixedHostRawUI>();
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
        ExpectWritePrefix();
        Expect(u => u.Write("a"));
        Expect(u => u.Write("b\n"));
        ExpectWritePrefix();
        Expect(u => u.Write("c"));

        UI.Write("a");
        UI.Write("b\n");
        UI.Write("c");
    }

    [Test]
    public void Write3()
    {
        ExpectWritePrefix(); Expect(u => u.Write(Yellow, DarkRed, "a"));
                             Expect(u => u.Write(Yellow, DarkRed, "b\n"));
        ExpectWritePrefix(); Expect(u => u.Write(Yellow, DarkRed, "c"));

        UI.Write(Yellow, DarkRed, "a");
        UI.Write(Yellow, DarkRed, "b\n");
        UI.Write(Yellow, DarkRed, "c");
    }

    [Test]
    public void WriteLine0()
    {
        ExpectWritePrefix(); Expect(u => u.WriteLine());
        ExpectWritePrefix(); Expect(u => u.Write("a"));
                             Expect(u => u.WriteLine());
        ExpectWritePrefix(); Expect(u => u.WriteLine());

        UI.WriteLine(); UI.Write("a");
        UI.WriteLine();
        UI.WriteLine();
    }

    [Test]
    public void WriteLine1()
    {
        ExpectWritePrefix(); Expect(u => u.WriteLine("a"));
        ExpectWritePrefix(); Expect(u => u.WriteLine("b"));

        UI.WriteLine("a\nb");
    }

    [Test]
    public void WriteLine3()
    {
        ExpectWritePrefix(); Expect(u => u.WriteLine(Yellow, DarkRed, "a"));
        ExpectWritePrefix(); Expect(u => u.WriteLine(Yellow, DarkRed, "b"));

        UI.WriteLine(Yellow, DarkRed, "a\r\nb");
    }

    [Test]
    public void WriteErrorLine([Values(null, "")] string? value)
    {
        UI.WriteErrorLine(value!);
    }

    [Test]
    public void WriteErrorLine()
    {
        InnerRawUI.SetupProperty(u => u.ForegroundColor);
        InnerRawUI.SetupProperty(u => u.BackgroundColor);

        ExpectWritePrefix(); Expect(u => u.WriteErrorLine("ERROR: a"));
        ExpectWritePrefix(); Expect(u => u.WriteErrorLine("ERROR: b"));
        ExpectWritePrefix(); Expect(u => u.WriteErrorLine("ERROR: c"));
        ExpectWritePrefix(); Expect(u => u.WriteErrorLine("ERROR: "));

        UI.WriteErrorLine("a\r\nb\nc\n");
    }

    [Test]
    public void WriteWarningLine()
    {
        ExpectWritePrefix(); Expect(u => u.WriteWarningLine("a"));
        ExpectWritePrefix(); Expect(u => u.WriteWarningLine("b"));
        ExpectWritePrefix(); Expect(u => u.WriteWarningLine("c"));

        UI.WriteWarningLine("a\r\nb\nc");
    }

    [Test]
    public void WriteVerboseLine()
    {
        ExpectWritePrefix(); Expect(u => u.WriteVerboseLine(""));
        ExpectWritePrefix(); Expect(u => u.WriteVerboseLine("a"));
        ExpectWritePrefix(); Expect(u => u.WriteVerboseLine("b"));

        UI.WriteVerboseLine("\na\r\nb");
    }

    [Test]
    public void WriteDebugLine()
    {
        ExpectWritePrefix(); Expect(u => u.WriteDebugLine("a"));
        ExpectWritePrefix(); Expect(u => u.WriteDebugLine("b"));

        UI.WriteDebugLine("a\nb");
    }

    [Test]
    public void WriteInformation()
    {
        var record = new InformationRecord("a", "b");

        Expect(u => u.WriteInformation(record));

        UI.WriteInformation(record);
    }

    [Test]
    public void WriteProgress()
    {
        var record = new ProgressRecord(1337, "a", "b");

        Expect(u => u.WriteProgress(42L, record));

        UI.WriteProgress(42L, record);
    }

    [Test]
    public void ReadLine()
    {
        Expect(u => u.ReadLine(), result: "a");

        UI.ReadLine().ShouldBe("a");
    }

    [Test]
    public void ReadLineAsSecureString()
    {
        using var s = new SecureString();

        s.AppendChar('a');
        s.MakeReadOnly();

        Expect(u => u.ReadLineAsSecureString(), result: s);

        UI.ReadLineAsSecureString().ShouldBeSameAs(s);
    }

    [Test]
    public void Prompt()
    {
        var descriptions = new Collection<FieldDescription>();
        var result       = new Dictionary<string, PSObject>();

        Expect(u => u.Prompt("a", "b", descriptions), result);

        UI.Prompt("a", "b", descriptions).ShouldBeSameAs(result);
    }

    [Test]
    public void PromptForChoice()
    {
        var descriptions = new Collection<ChoiceDescription>();

        Expect(u => u.PromptForChoice("a", "b", descriptions, 3), result: 2);

        UI.PromptForChoice("a", "b", descriptions, 3).ShouldBe(2);
    }

    [Test]
    public void PromptForCredential4()
    {
        using var password = MakeSecureString('p');

        var credential = new PSCredential("u", password);

        Expect(u => u.PromptForCredential("a", "b", "c", "d"), result: credential);

        UI.PromptForCredential("a", "b", "c", "d").ShouldBeSameAs(credential);
    }

    [Test]
    public void PromptForCredential6()
    {
        using var password = MakeSecureString('p');

        var credential = new PSCredential("u", password);
        var types      = PSCredentialTypes.Generic;
        var options    = PSCredentialUIOptions.AlwaysPrompt;

        Expect(u => u.PromptForCredential("a", "b", "c", "d", types, options), result: credential);

        UI.PromptForCredential("a", "b", "c", "d", types, options).ShouldBeSameAs(credential);
    }

    protected override PSHostUserInterface CreateHostUI(PSHostUserInterface ui)
        => new PrefixedHostUI(ui, Prefix.Object);

    private static SecureString MakeSecureString(char c)
    {
        var secureString = new SecureString();

        secureString.AppendChar(c);
        secureString.MakeReadOnly();

        return secureString;
    }

    private void ExpectWritePrefix()
    {
        Prefix
            .InSequence(Sequence)
            .Setup(p => p.Write(InnerUI.Object))
            .Verifiable();
    }
}
