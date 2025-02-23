// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

[TestFixture]
public class StringPrefixTests
{
    [Test]
    public void Construct_NullPrefix()
    {
        Should.Throw<ArgumentNullException>(
            () => new StringPrefix(null!)
        );
    }

    [Test]
    public void Colors_Get()
    {
        var prefix = new StringPrefix("a");

        prefix.ForegroundColor.ShouldBe(ConsoleColor.DarkBlue);
        prefix.BackgroundColor.ShouldBe(ConsoleColor.Black);
    }

    [Test]
    public void GetPrefix()
    {
        new StringPrefix("a").GetPrefix().ShouldBe("[a] ");
    }
}
