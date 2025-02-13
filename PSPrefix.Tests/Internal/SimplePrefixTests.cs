// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

[TestFixture]
public class SimplePrefixTests
{
    [Test]
    public void Length_Get()
    {
        var prefix = new TestPrefix();

        prefix.Length.Should().Be(4); // "test".Length
    }

    [Test]
    public void ForegroundColor_Get()
    {
        var prefix = new TestPrefix();

        prefix.ForegroundColor.Should().Be(ConsoleColor.DarkGray);
    }

    [Test]
    public void ForegroundColor_Set()
    {
        var prefix = new TestPrefix() { ForegroundColor = ConsoleColor.Green };

        prefix.ForegroundColor.Should().Be(ConsoleColor.Green);
    }

    [Test]
    public void BackgroundColor_Get()
    {
        var prefix = new TestPrefix();

        prefix.BackgroundColor.Should().Be(ConsoleColor.Black);
    }

    [Test]
    public void BackgroundColor_Set()
    {
        var prefix = new TestPrefix() { BackgroundColor = ConsoleColor.DarkGray };

        prefix.BackgroundColor.Should().Be(ConsoleColor.DarkGray);
    }

    [Test]
    public void Write()
    {
        var prefix = new TestPrefix()
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkGray,
        };

        static void AssertArgs(ConsoleColor fg, ConsoleColor bg, string s)
        {
            fg.Should().Be(ConsoleColor.Green);
            bg.Should().Be(ConsoleColor.DarkGray);
            s .Should().Be("test");
        }

        var ui = Mock.Of<PSHostUserInterface>(MockBehavior.Strict);

        Mock.Get(ui)
            .Setup(u => u.Write(
                It.IsAny<ConsoleColor>(),
                It.IsAny<ConsoleColor>(),
                It.IsAny<string>()
            ))
            .Callback(AssertArgs)
            .Verifiable();

        prefix.Write(ui);
    }

    private class TestPrefix : SimplePrefix
    {
        protected internal override string GetPrefix() => "test";
    }
}
