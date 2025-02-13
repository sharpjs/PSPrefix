// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Linq.Expressions;

namespace PSPrefix.Internal;

public abstract class PSHostRawUITests : TestHarnessBase
{
    private PSHostRawUserInterface? _rawUI;

    protected PSHostRawUserInterface RawUI => _rawUI ??= CreateHostUI(InnerRawUI.Object);

    protected Mock<PSHostRawUserInterface> InnerRawUI { get; }

    protected PSHostRawUITests()
    {
        InnerRawUI = Mocks.Create<PSHostRawUserInterface>();
    }

    [Test]
    public void LengthInBufferCells_Char()
    {
        Expect(u => u.LengthInBufferCells('a'), result: 1);

        RawUI.LengthInBufferCells('a').Should().Be(1);
    }

    [Test]
    public void LengthInBufferCells_String()
    {
        Expect(u => u.LengthInBufferCells("abc"), result: 3);

        RawUI.LengthInBufferCells("abc").Should().Be(3);
    }

    [Test]
    public void LengthInBufferCells_StringOffset()
    {
        Expect(u => u.LengthInBufferCells("abc", 1), result: 2);

        RawUI.LengthInBufferCells("abc", 1).Should().Be(2);
    }

    protected abstract PSHostRawUserInterface CreateHostUI(PSHostRawUserInterface rawUI);

    protected void ExpectSet(Action<PSHostRawUserInterface> expression)
    {
        InnerRawUI
            .SetupSet(expression)
            .Verifiable();
    }

    protected void Expect(Expression<Action<PSHostRawUserInterface>> expression)
    {
        InnerRawUI
            .Setup(expression)
            .Verifiable();
    }

    protected void Expect<T>(Expression<Func<PSHostRawUserInterface, T>> expression, T result)
    {
        InnerRawUI
            .Setup(expression)
            .Returns(result)
            .Verifiable();
    }
}
