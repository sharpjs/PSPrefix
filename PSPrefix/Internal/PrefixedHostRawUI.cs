// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

internal class PrefixedHostRawUI : PSHostRawUserInterface
{
    private readonly PSHostRawUserInterface _rawUI;
    private readonly IPrefix                _prefix;

    public PrefixedHostRawUI(PSHostRawUserInterface rawUI, IPrefix prefix)
    {
        _rawUI  = rawUI;
        _prefix = prefix;
    }

    private int Margin => _prefix.Length;

    /// <inheritdoc/>
    public override ConsoleColor ForegroundColor
    {
        get => _rawUI.ForegroundColor;
        set => _rawUI.ForegroundColor = value;
    }

    /// <inheritdoc/>
    public override ConsoleColor BackgroundColor
    {
        get => _rawUI.BackgroundColor;
        set => _rawUI.BackgroundColor = value;
    }

    /// <inheritdoc/>
    public override Coordinates WindowPosition
    {
        get => _rawUI.WindowPosition;
        set => _rawUI.WindowPosition = value;
    }

    /// <inheritdoc/>
    public override Size WindowSize // in character cells
    {
        get => ToVirtual(_rawUI.WindowSize);
        set => _rawUI.WindowSize = ToReal(value);
    }

    /// <inheritdoc/>
    public override Size MaxWindowSize // in character cells
    {
        get => ToVirtual(_rawUI.MaxWindowSize);
    }

    /// <inheritdoc/>
    public override Size MaxPhysicalWindowSize // in character cells
    {
        get => ToVirtual(_rawUI.MaxPhysicalWindowSize);
    }

    /// <inheritdoc/>
    public override string WindowTitle
    {
        get => _rawUI.WindowTitle;
        set => _rawUI.WindowTitle = value;
    }

    /// <inheritdoc/>
    public override Size BufferSize // in character cells
    {
        get => ToVirtual(_rawUI.BufferSize);
        set => _rawUI.BufferSize = ToReal(value);
    }

    /// <inheritdoc/>
    public override Coordinates CursorPosition // in character cells
    {
        get => ToVirtual(_rawUI.CursorPosition);
        set => _rawUI.CursorPosition = ToReal(value);
    }

    /// <inheritdoc/>
    public override int CursorSize // in percent
    {
        get => _rawUI.CursorSize;
        set => _rawUI.CursorSize = value;
    }

    /// <inheritdoc/>
    public override bool KeyAvailable
    {
        get => _rawUI.KeyAvailable;
    }

    /// <inheritdoc/>
    public override BufferCell[,] GetBufferContents(Rectangle rectangle)
    {
        return _rawUI.GetBufferContents(ToReal(rectangle));
    }

    /// <inheritdoc/>
    public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
    {
        _rawUI.ScrollBufferContents(ToReal(source), ToReal(destination), ToReal(clip), fill);
    }

    /// <inheritdoc/>
    public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
    {
        _rawUI.SetBufferContents(ToReal(origin), contents);
    }

    /// <inheritdoc/>
    public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
    {
        _rawUI.SetBufferContents(ToReal(rectangle), fill);
    }

    /// <inheritdoc/>
    public override int LengthInBufferCells(char source)
    {
        return _rawUI.LengthInBufferCells(source);
    }

    /// <inheritdoc/>
    public override int LengthInBufferCells(string source)
    {
        return _rawUI.LengthInBufferCells(source);
    }

    /// <inheritdoc/>
    public override int LengthInBufferCells(string source, int offset)
    {
        return _rawUI.LengthInBufferCells(source, offset);
    }

    /// <inheritdoc/>
    public override KeyInfo ReadKey(ReadKeyOptions options)
    {
        return _rawUI.ReadKey(options);
    }

    /// <inheritdoc/>
    public override void FlushInputBuffer()
    {
        _rawUI.FlushInputBuffer();
    }

    private Size ToVirtual(Size size)
    {
        return new(size.Width - Margin, size.Height);
    }

    private Size ToReal(Size size)
    {
        return new(size.Width + Margin, size.Height);
    }

    private Coordinates ToVirtual(Coordinates point)
    {
        return new(point.X - Margin, point.Y);
    }

    private Coordinates ToReal(Coordinates point)
    {
        return new(point.X + Margin, point.Y);
    }

#if EVER_NEEDED
    private Rectangle ToVirtual(Rectangle rectangle)
    {
        return new(
            rectangle.Left  - _margin, rectangle.Top,
            rectangle.Right - _margin, rectangle.Bottom
        );
    }
#endif

    private Rectangle ToReal(Rectangle rectangle)
    {
        return new(
            rectangle.Left  + Margin, rectangle.Top,
            rectangle.Right + Margin, rectangle.Bottom
        );
    }
}
