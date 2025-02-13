// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

[TestFixture]
public class StringPrefixTests
{
    [Test]
    public void Construct_NullPrefix()
    {
        Invoking(() => new StringPrefix(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Colors_Get()
    {
        var prefix = new StringPrefix("a");

        prefix.ForegroundColor.Should().Be(ConsoleColor.DarkBlue);
        prefix.BackgroundColor.Should().Be(ConsoleColor.Black);
    }

    [Test]
    public void GetPrefix()
    {
        new StringPrefix("a").GetPrefix().Should().Be("[a] ");
    }
}
