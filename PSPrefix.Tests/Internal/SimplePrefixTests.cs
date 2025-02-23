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

        prefix.Length.ShouldBe(4); // "test".Length
    }

    [Test]
    public void ForegroundColor_Get()
    {
        var prefix = new TestPrefix();

        prefix.ForegroundColor.ShouldBe(ConsoleColor.DarkGray);
    }

    [Test]
    public void ForegroundColor_Set()
    {
        var prefix = new TestPrefix() { ForegroundColor = ConsoleColor.Green };

        prefix.ForegroundColor.ShouldBe(ConsoleColor.Green);
    }

    [Test]
    public void BackgroundColor_Get()
    {
        var prefix = new TestPrefix();

        prefix.BackgroundColor.ShouldBe(ConsoleColor.Black);
    }

    [Test]
    public void BackgroundColor_Set()
    {
        var prefix = new TestPrefix() { BackgroundColor = ConsoleColor.DarkGray };

        prefix.BackgroundColor.ShouldBe(ConsoleColor.DarkGray);
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
            fg.ShouldBe(ConsoleColor.Green);
            bg.ShouldBe(ConsoleColor.DarkGray);
            s .ShouldBe("test");
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
