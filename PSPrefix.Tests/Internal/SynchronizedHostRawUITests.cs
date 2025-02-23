// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

using static ConsoleColor;

[TestFixture]
public class SynchronizedHostRawUITests : PSHostRawUITests
{
    [Test]
    public void ForegroundColor_Get()
    {
        Expect(u => u.ForegroundColor, result: Green);

        RawUI.ForegroundColor.ShouldBe(Green);
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

        RawUI.BackgroundColor.ShouldBe(DarkBlue);
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
        var value = new Coordinates(42, 13);

        Expect(u => u.WindowPosition, result: value);

        RawUI.WindowPosition.ShouldBe(value);
    }

    [Test]
    public void WindowPosition_Set()
    {
        var value = new Coordinates(42, 13);

        ExpectSet(u => u.WindowPosition = value);

        RawUI.WindowPosition = value;
    }

    [Test]
    public void WindowSize_Get()
    {
        var value = new Size(42, 13);

        Expect(u => u.WindowSize, result: value);

        RawUI.WindowSize.ShouldBe(value);
    }

    [Test]
    public void WindowSize_Set()
    {
        var value = new Size(42, 13);

        ExpectSet(u => u.WindowSize = value);

        RawUI.WindowSize = value;
    }

    [Test]
    public void MaxWindowSize_Get()
    {
        var value = new Size(42, 13);

        Expect(u => u.MaxWindowSize, result: value);

        RawUI.MaxWindowSize.ShouldBe(value);
    }

    [Test]
    public void MaxPhysicalWindowSize_Get()
    {
        var value = new Size(42, 13);

        Expect(u => u.MaxPhysicalWindowSize, result: value);

        RawUI.MaxPhysicalWindowSize.ShouldBe(value);
    }

    [Test]
    public void WindowTitle_Get()
    {
        Expect(u => u.WindowTitle, result: "a");

        RawUI.WindowTitle.ShouldBe("a");
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
        var value = new Size(42, 13);

        Expect(u => u.BufferSize, result: value);

        RawUI.BufferSize.ShouldBe(value);
    }

    [Test]
    public void BufferSize_Set()
    {
        var value = new Size(42, 13);

        ExpectSet(u => u.BufferSize = value);

        RawUI.BufferSize = value;
    }

    [Test]
    public void CursorPosition_Get()
    {
        var value = new Coordinates(42, 13);

        Expect(u => u.CursorPosition, result: value);

        RawUI.CursorPosition.ShouldBe(value);
    }

    [Test]
    public void CursorPosition_Set()
    {
        var value = new Coordinates(42, 13);

        ExpectSet(u => u.CursorPosition = value);

        RawUI.CursorPosition = value;
    }

    [Test]
    public void CursorSize_Get()
    {
        Expect(u => u.CursorSize, result: 3);

        RawUI.CursorSize.ShouldBe(3);
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

        RawUI.KeyAvailable.ShouldBeTrue();
    }

    [Test]
    public void GetBufferContents()
    {
        var rectangle = new Rectangle(1, 2, 3, 4);
        var result    = new BufferCell[2, 2];

        Expect(u => u.GetBufferContents(rectangle), result);

        RawUI.GetBufferContents(rectangle).ShouldBe(result);
    }

    [Test]
    public void ScrollBufferContents()
    {
        var source      = new Rectangle(1, 2, 3, 4);
        var destination = new Coordinates(5, 6);
        var clip        = new Rectangle(7, 8, 9, 10);
        var fill        = new BufferCell(' ', White, Black, default);

        Expect(u => u.ScrollBufferContents(source, destination, clip, fill));

        RawUI.ScrollBufferContents(source, destination, clip, fill);
    }

    [Test]
    public void SetBufferContents_Array()
    {
        var origin   = new Coordinates(1, 2);
        var contents = new BufferCell[2, 2];

        Expect(u => u.SetBufferContents(origin, contents));

        RawUI.SetBufferContents(origin, contents);
    }

    [Test]
    public void SetBufferContents_Fill()
    {
        var rectangle = new Rectangle(1, 2, 3, 4);
        var fill      = new BufferCell(' ', White, Black, default);

        Expect(u => u.SetBufferContents(rectangle, fill));

        RawUI.SetBufferContents(rectangle, fill);
    }

    [Test]
    public void ReadKey()
    {
        var options = ReadKeyOptions.IncludeKeyDown;
        var value   = new KeyInfo(42, 'A', ControlKeyStates.ShiftPressed, true);

        Expect(u => u.ReadKey(options), result: value);

        RawUI.ReadKey(options).ShouldBe(value);
    }

    [Test]
    public void FlushInputBuffer()
    {
        Expect(u => u.FlushInputBuffer());

        RawUI.FlushInputBuffer();
    }

    protected override PSHostRawUserInterface CreateHostUI(PSHostRawUserInterface rawUI)
        => new SynchronizedHostRawUI(rawUI, @lock: new());
}
