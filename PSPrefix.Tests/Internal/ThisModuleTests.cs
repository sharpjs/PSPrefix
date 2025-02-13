// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

[TestFixture]
public class ThisModuleTests
{
    [Test]
    public void Version()
    {
        ThisModule.Version.Should().NotBeNull();
    }
}
