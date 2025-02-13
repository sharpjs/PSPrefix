// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Globalization;

namespace PSPrefix.Internal;

public abstract class PSHostTests : TestHarnessBase
{
    private PSHost? _host;

    protected PSHost Host => _host ??= CreateHost(InnerHost.Object);

    protected Mock<PSHost>                 InnerHost  { get; }
    protected Mock<PSHostUserInterface>    InnerUI    { get; }
    protected Mock<PSHostRawUserInterface> InnerRawUI { get; }

    protected PSHostTests()
    {
        InnerHost  = Mocks.Create<PSHost>();
        InnerUI    = Mocks.Create<PSHostUserInterface>();
        InnerRawUI = Mocks.Create<PSHostRawUserInterface>();

        InnerUI.Setup(u => u.RawUI).Returns(InnerRawUI.Object);

        InnerHost.Setup(h => h.Name).Returns("MockHost");
        InnerHost.Setup(h => h.UI  ).Returns(InnerUI.Object);
    }

    [Test]
    public void Construct_NullHost()
    {
        Invoking(() => CreateHost(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void InstanceId_Get()
    {
        Host.InstanceId.Should().NotBeEmpty();
    }

    [Test]
    public void Version_Get()
    {
        Host.Version.Should().Be(ThisModule.Version);
    }

    [Test]
    public void UI_Get_Null()
    {
        InnerHost.Setup(h => h.UI).Returns(default(PSHostUserInterface)!);

        Host.UI.Should().BeNull();
    }

    [Test]
    public void DebuggerEnabled_Get([Values(false, true)] bool value)
    {
        InnerHost
            .Setup(h => h.DebuggerEnabled)
            .Returns(value)
            .Verifiable();

        Host.DebuggerEnabled.Should().Be(value);
    }

    [Test]
    public void DebuggerEnabled_Set([Values(false, true)] bool value)
    {
        InnerHost
            .SetupSet(h => h.DebuggerEnabled = value)
            .Verifiable();

        Host.DebuggerEnabled = value;
    }

    [Test]
    public void CurrentCulture_Get()
    {
        var expected = CultureInfo.GetCultureInfo("kl-GL");

        InnerHost
            .Setup(h => h.CurrentCulture)
            .Returns(expected)
            .Verifiable();

        Host.CurrentCulture.Should().BeSameAs(expected);
    }

    [Test]
    public void CurrentUICulture_Get()
    {
        var expected = CultureInfo.GetCultureInfo("kl-GL");

        InnerHost
            .Setup(h => h.CurrentUICulture)
            .Returns(expected)
            .Verifiable();

        Host.CurrentUICulture.Should().BeSameAs(expected);
    }

    [Test]
    public void PrivateData_Get()
    {
        var expected = new PSObject("test private data");

        InnerHost
            .Setup(h => h.PrivateData)
            .Returns(expected)
            .Verifiable();

        Host.PrivateData.Should().BeSameAs(expected);
    }

    [Test]
    public void EnterNestedPrompt()
    {
        InnerHost
            .Setup(h => h.EnterNestedPrompt())
            .Verifiable();

        Host.EnterNestedPrompt();
    }

    [Test]
    public void ExitNestedPrompt()
    {
        InnerHost
            .Setup(h => h.ExitNestedPrompt())
            .Verifiable();

        Host.ExitNestedPrompt();
    }

    [Test]
    public void NotifyBeginApplication()
    {
        InnerHost
            .Setup(h => h.NotifyBeginApplication())
            .Verifiable();

        Host.NotifyBeginApplication();
    }

    [Test]
    public void NotifyEndApplication()
    {
        InnerHost
            .Setup(h => h.NotifyEndApplication())
            .Verifiable();

        Host.NotifyEndApplication();
    }

    [Test]
    public void SetShouldExit()
    {
        var code = Random.Next();

        InnerHost
            .Setup(h => h.SetShouldExit(code))
            .Verifiable();

        Host.SetShouldExit(code);
    }

    protected abstract PSHost CreateHost(PSHost host);
}
