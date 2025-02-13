// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

using static ConsoleColor;

[TestFixture]
public class PrefixedHostRawUITests : PSHostRawUITests
{
    private const int Inset = 7;

    private new PrefixedHostRawUI RawUI => (PrefixedHostRawUI) base.RawUI;

    private Mock<IPrefix> Prefix { get; }

    public PrefixedHostRawUITests()
    {
        Prefix = Mocks.Create<IPrefix>();

        Prefix.Setup(p => p.Length).Returns(Inset);
    }

    [Test]
    public void ForegroundColor_Get()
    {
        Expect(u => u.ForegroundColor, result: Green);

        RawUI.ForegroundColor.Should().Be(Green);
    }

    [Test]
    public void ForegroundColor_Set()
    {
        ExpectSet(u => u.ForegroundColor = Green);

        RawUI.ForegroundColor = Green;
    }

    [Test]
    public void BackgroundColor_Get()
    {
        Expect(u => u.BackgroundColor, result: DarkBlue);

        RawUI.BackgroundColor.Should().Be(DarkBlue);
    }

    [Test]
    public void BackgroundColor_Set()
    {
        ExpectSet(u => u.BackgroundColor = DarkBlue);

        RawUI.BackgroundColor = DarkBlue;
    }

    [Test]
    public void WindowPosition_Get()
    {
        var value = new Coordinates(1, 2);

        Expect(u => u.WindowPosition, result: value);

        RawUI.WindowPosition.Should().Be(value);
    }

    [Test]
    public void WindowPosition_Set()
    {
        var value = new Coordinates(1, 2);

        ExpectSet(u => u.WindowPosition = value);

        RawUI.WindowPosition = value;
    }

    [Test]
    public void WindowSize_Get()
    {
        var innerValue = new Size(80,         50); // in character cells
        var outerValue = new Size(80 - Inset, 50); // in character cells

        Expect(u => u.WindowSize, result: innerValue);

        RawUI.WindowSize.Should().Be(outerValue);
    }

    [Test]
    public void WindowSize_Set()
    {
        var innerValue = new Size(80,         50); // in character cells
        var outerValue = new Size(80 - Inset, 50); // in character cells

        ExpectSet(u => u.WindowSize = innerValue);

        RawUI.WindowSize = outerValue;
    }

    [Test]
    public void MaxWindowSize_Get()
    {
        var innerValue = new Size(80,         50); // in character cells
        var outerValue = new Size(80 - Inset, 50); // in character cells

        Expect(u => u.MaxWindowSize, result: innerValue);

        RawUI.MaxWindowSize.Should().Be(outerValue);
    }

    [Test]
    public void MaxPhysicalWindowSize_Get()
    {
        var innerValue = new Size(80,         50); // in character cells
        var outerValue = new Size(80 - Inset, 50); // in character cells

        Expect(u => u.MaxPhysicalWindowSize, result: innerValue);

        RawUI.MaxPhysicalWindowSize.Should().Be(outerValue);
    }

    [Test]
    public void WindowTitle_Get()
    {
        Expect(u => u.WindowTitle, result: "a");

        RawUI.WindowTitle.Should().Be("a");
    }

    [Test]
    public void WindowTitle_Set()
    {
        ExpectSet(u => u.WindowTitle = "a");

        RawUI.WindowTitle = "a";
    }

    [Test]
    public void BufferSize_Get()
    {
        var innerValue = new Size(80,         50); // in character cells
        var outerValue = new Size(80 - Inset, 50); // in character cells

        Expect(u => u.BufferSize, result: innerValue);

        RawUI.BufferSize.Should().Be(outerValue);
    }

    [Test]
    public void BufferSize_Set()
    {
        var innerValue = new Size(80,         50); // in character cells
        var outerValue = new Size(80 - Inset, 50); // in character cells

        ExpectSet(u => u.BufferSize = innerValue);

        RawUI.BufferSize = outerValue;
    }

    [Test]
    public void CursorPosition_Get()
    {
        var innerValue = new Coordinates(40,         13); // in character cells
        var outerValue = new Coordinates(40 - Inset, 13); // in character cells

        Expect(u => u.CursorPosition, result: innerValue);

        RawUI.CursorPosition.Should().Be(outerValue);
    }

    [Test]
    public void CursorPosition_Set()
    {
        var innerValue = new Coordinates(40,         13); // in character cells
        var outerValue = new Coordinates(40 - Inset, 13); // in character cells

        ExpectSet(u => u.CursorPosition = innerValue);

        RawUI.CursorPosition = outerValue;
    }

    [Test]
    public void CursorSize_Get()
    {
        Expect(u => u.CursorSize, result: 3);

        RawUI.CursorSize.Should().Be(3);
    }

    [Test]
    public void CursorSize_Set()
    {
        ExpectSet(u => u.CursorSize = 3);

        RawUI.CursorSize = 3;
    }

    [Test]
    public void KeyAvailable_Set()
    {
        Expect(u => u.KeyAvailable, result: true);

        RawUI.KeyAvailable.Should().BeTrue();
    }

    [Test]
    public void GetBufferContents()
    {
        var innerRectangle = new Rectangle(1 + Inset, 2, 3 + Inset, 4);
        var outerRectangle = new Rectangle(1,         2, 3,         4);
        var result         = new BufferCell[2, 2];

        Expect(u => u.GetBufferContents(innerRectangle), result);

        RawUI.GetBufferContents(outerRectangle).Should().Be(result);
    }

    [Test]
    public void ScrollBufferContents()
    {
        var innerSource      = new Rectangle  (1 + Inset, 2, 3 + Inset, 4);
        var outerSource      = new Rectangle  (1,         2, 3,         4);
        var innerDestination = new Coordinates(5 + Inset, 6);
        var outerDestination = new Coordinates(5,         6);
        var innerClip        = new Rectangle  (7 + Inset, 8, 9 + Inset, 10);
        var outerClip        = new Rectangle  (7,         8, 9,         10);
        var fill             = new BufferCell(' ', White, Black, default);

        Expect(u => u.ScrollBufferContents(innerSource, innerDestination, innerClip, fill));

        RawUI.ScrollBufferContents(outerSource, outerDestination, outerClip, fill);
    }

    [Test]
    public void SetBufferContents_Array()
    {
        var innerOrigin = new Coordinates(1 + Inset, 2);
        var outerOrigin = new Coordinates(1,         2);
        var contents    = new BufferCell[2, 2];

        Expect(u => u.SetBufferContents(innerOrigin, contents));

        RawUI.SetBufferContents(outerOrigin, contents);
    }

    [Test]
    public void SetBufferContents_Fill()
    {
        var innerRectangle = new Rectangle(1 + Inset, 2, 3 + Inset, 4);
        var outerRectangle = new Rectangle(1,         2, 3,         4);
        var fill           = new BufferCell(' ', White, Black, default);

        Expect(u => u.SetBufferContents(innerRectangle, fill));

        RawUI.SetBufferContents(outerRectangle, fill);
    }

    [Test]
    public void ReadKey()
    {
        var options = ReadKeyOptions.IncludeKeyDown;
        var value   = new KeyInfo(42, 'A', ControlKeyStates.ShiftPressed, true);

        Expect(u => u.ReadKey(options), result: value);

        RawUI.ReadKey(options).Should().Be(value);
    }

    [Test]
    public void FlushInputBuffer()
    {
        Expect(u => u.FlushInputBuffer());

        RawUI.FlushInputBuffer();
    }

    protected override PSHostRawUserInterface CreateHostUI(PSHostRawUserInterface rawUI)
        => new PrefixedHostRawUI(rawUI, Prefix.Object);
}
