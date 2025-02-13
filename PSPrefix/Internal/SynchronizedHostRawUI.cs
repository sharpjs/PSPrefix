// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

public class SynchronizedHostRawUI : PSHostRawUserInterface
{
    private readonly PSHostRawUserInterface _rawUI;
    private readonly object                 _lock;

    internal SynchronizedHostRawUI(PSHostRawUserInterface rawUI, object @lock)
    {
        _rawUI = rawUI;
        _lock  = @lock;
    }

    /// <inheritdoc/>
    public override ConsoleColor ForegroundColor
    {
        get { lock (_lock) return _rawUI.ForegroundColor; }
        set { lock (_lock) _rawUI.ForegroundColor = value; }
    }

    /// <inheritdoc/>
    public override ConsoleColor BackgroundColor
    {
        get { lock (_lock) return _rawUI.BackgroundColor; }
        set { lock (_lock) _rawUI.BackgroundColor = value; }
    }

    /// <inheritdoc/>
    public override Coordinates WindowPosition
    {
        get { lock (_lock) return _rawUI.WindowPosition; }
        set { lock (_lock) _rawUI.WindowPosition = value; }
    }

    /// <inheritdoc/>
    public override Size WindowSize // in character cells
    {
        get { lock (_lock) return _rawUI.WindowSize; }
        set { lock (_lock) _rawUI.WindowSize = value; }
    }

    /// <inheritdoc/>
    public override Size MaxWindowSize // in character cells
    {
        get { lock (_lock) return _rawUI.MaxWindowSize; }
    }

    /// <inheritdoc/>
    public override Size MaxPhysicalWindowSize // in character cells
    {
        get { lock (_lock) return _rawUI.MaxPhysicalWindowSize; }
    }

    /// <inheritdoc/>
    public override string WindowTitle
    {
        get { lock (_lock) return _rawUI.WindowTitle; }
        set { lock (_lock) _rawUI.WindowTitle = value; }
    }

    /// <inheritdoc/>
    public override Size BufferSize // in character cells
    {
        get { lock (_lock) return _rawUI.BufferSize; }
        set { lock (_lock) _rawUI.BufferSize = value; }
    }

    /// <inheritdoc/>
    public override Coordinates CursorPosition // in character cells
    {
        get { lock (_lock) return _rawUI.CursorPosition; }
        set { lock (_lock) _rawUI.CursorPosition = value; }
    }

    /// <inheritdoc/>
    public override int CursorSize // in percent
    {
        get { lock (_lock) return _rawUI.CursorSize; }
        set { lock (_lock) _rawUI.CursorSize = value; }
    }

    /// <inheritdoc/>
    public override bool KeyAvailable
    {
        get { lock (_lock) return _rawUI.KeyAvailable; }
    }

    /// <inheritdoc/>
    public override BufferCell[,] GetBufferContents(Rectangle rectangle)
    {
        lock (_lock) return _rawUI.GetBufferContents(rectangle);
    }

    /// <inheritdoc/>
    public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
    {
        lock (_lock) _rawUI.SetBufferContents(rectangle, fill);
    }

    /// <inheritdoc/>
    public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
    {
        lock (_lock) _rawUI.SetBufferContents(origin, contents);
    }

    /// <inheritdoc/>
    public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
    {
        lock (_lock) _rawUI.ScrollBufferContents(source, destination, clip, fill);
    }

    /// <inheritdoc/>
    public override int LengthInBufferCells(char source)
    {
        lock (_lock) return _rawUI.LengthInBufferCells(source);
    }

    /// <inheritdoc/>
    public override int LengthInBufferCells(string source)
    {
        lock (_lock) return _rawUI.LengthInBufferCells(source);
    }

    /// <inheritdoc/>
    public override int LengthInBufferCells(string source, int offset)
    {
        lock (_lock) return _rawUI.LengthInBufferCells(source, offset);
    }

    /// <inheritdoc/>
    public override KeyInfo ReadKey(ReadKeyOptions options)
    {
        lock (_lock) return _rawUI.ReadKey(options);
    }

    /// <inheritdoc/>
    public override void FlushInputBuffer()
    {
        lock (_lock) _rawUI.FlushInputBuffer();
    }
}
