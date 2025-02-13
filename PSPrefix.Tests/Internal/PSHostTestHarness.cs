namespace PSPrefix.Internal;

internal class PSHostTestHarness : TestHarnessBase
{
    protected Mock<PSHost>                 Host  { get; }
    protected Mock<PSHostUserInterface>    UI    { get; }
    protected Mock<PSHostRawUserInterface> RawUI { get; }

    public PSHostTestHarness()
    {
        Host  = Mocks.Create<PSHost>();
        UI    = Mocks.Create<PSHostUserInterface>();
        RawUI = Mocks.Create<PSHostRawUserInterface>();

        UI.Setup(u => u.RawUI).Returns(RawUI.Object);

        Host.Setup(h => h.Name).Returns("MockHost");
        Host.Setup(h => h.UI  ).Returns(UI.Object);
    }
}
