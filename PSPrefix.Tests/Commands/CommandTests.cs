// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Commands;

public abstract class CommandTests : TestHarnessBase
{
    protected Mock<PSHost>                 Host  { get; }
    protected Mock<PSHostUserInterface>    UI    { get; }
    protected Mock<PSHostRawUserInterface> RawUI { get; }

    protected CommandTests()
    {
        Host  = Mocks.Create<PSHost>();
        UI    = Mocks.Create<PSHostUserInterface>();
        RawUI = Mocks.Create<PSHostRawUserInterface>();

        UI.Setup(u => u.RawUI).Returns(RawUI.Object);

        Host.Setup(h => h.UI).Returns(UI.Object);
    }
}
