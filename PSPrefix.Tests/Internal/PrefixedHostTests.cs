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
        Invoking(() => CreatePrefixedHost(InnerHost.Object, null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Name_Get()
    {
        Host.Name.Should().Be("PrefixedHost(MockHost)");
    }

    [Test]
    public void UI_Get_NotNull()
    {
        Host.UI.Should().BeOfType<PrefixedHostUI>();
    }

    private PrefixedHost CreatePrefixedHost(PSHost host, IPrefix prefix)
        => new PrefixedHost(host, prefix);

    protected override PSHost CreateHost(PSHost host)
        => CreatePrefixedHost(host, Prefix.Object);
}
