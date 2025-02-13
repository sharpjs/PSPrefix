// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

[TestFixture]
public class SynchronizedHostTests : PSHostTests
{
    [Test]
    public void Name_Get()
    {
        Host.Name.Should().Be("SynchronizedHost(MockHost)");
    }

    [Test]
    public void UI_Get_NotNull()
    {
        Host.UI.Should().BeOfType<SynchronizedHostUI>();
    }

    protected override PSHost CreateHost(PSHost host)
        => new SynchronizedHost(host);
}
