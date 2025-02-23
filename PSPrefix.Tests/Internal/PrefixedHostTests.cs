// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

[TestFixture]
public class PrefixedHostTests : PSHostTests
{
    private Mock<IPrefix> Prefix { get; }

    public PrefixedHostTests()
    {
        Prefix = Mocks.Create<IPrefix>();
    }

    [Test]
    public void Construct_NullPrefix()
    {
        Should.Throw<ArgumentNullException>(
            () => CreatePrefixedHost(InnerHost.Object, null!)
        );
    }

    [Test]
    public void Name_Get()
    {
        Host.Name.ShouldBe("PrefixedHost(MockHost)");
    }

    [Test]
    public void UI_Get_NotNull()
    {
        Host.UI.ShouldBeOfType<PrefixedHostUI>();
    }

    private static PrefixedHost CreatePrefixedHost(PSHost host, IPrefix prefix)
        => new(host, prefix);

    protected override PSHost CreateHost(PSHost host)
        => CreatePrefixedHost(host, Prefix.Object);
}
