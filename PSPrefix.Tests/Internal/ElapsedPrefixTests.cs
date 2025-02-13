// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

[TestFixture]
public class ElapsedPrefixTests
{
    [Test]
    public void GetPrefix()
    {
        var prefix = new ElapsedPrefix();

        prefix.GetPrefix().Should().MatchRegex(@"^\[\+[0-9]{2}:[0-9]{2}:[0-9]{2}\] $");
    }

    [Test]
    [Retry(3)] // for slight race condition
    public void GetPrefix_Cached()
    {
        var prefix = new ElapsedPrefix();

        var a = prefix.GetPrefix();
        var b = prefix.GetPrefix(); // slight race condition here

        a.Should().BeSameAs(b);
    }

    [Test]
    public async Task GetPrefix_NotCached()
    {
        var prefix = new ElapsedPrefix();

        var a = prefix.GetPrefix();
        await Task.Delay(millisecondsDelay: 1100);
        var b = prefix.GetPrefix();

        a.Should().NotBeSameAs(b);
    }

    [Test]
    [TestCase("01:23:45",     "01:23:45")]
    [TestCase("01:23:45.999", "01:23:45")]
    public void Floor(TimeSpan input, TimeSpan output)
    {
        ElapsedPrefix.Floor(input).Should().Be(output);
    }
}
