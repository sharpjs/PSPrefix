// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Globalization;

namespace PSPrefix.Internal;

/// <summary>
///   A PowerShell host wrapper that is safe to share across threads.
/// </summary>
/// <remarks>
///   All members of this class are thread-safe.
/// </remarks>
public class SynchronizedHost : PSHost
{
    private readonly PSHost              _host;
    private readonly SynchronizedHostUI? _ui;
    private readonly object              _lock;
    private readonly string              _name;
    private readonly Guid                _id;

    /// <summary>
    ///   Initializes a new <see cref="SynchronizedHost"/> instance.
    /// </summary>
    /// <param name="host">
    ///   The underlying host.
    /// </param>
    public SynchronizedHost(PSHost host)
    {
        if (host is null)
            throw new ArgumentNullException(nameof(host));

        var @lock = new object();

        _host = host;
        _ui   = host.UI is { } ui ? new(ui, @lock) : null;
        _lock = @lock;
        _name = $"{nameof(SynchronizedHost)}({host.Name})";
        _id   = Guid.NewGuid();
    }

    /// <inheritdoc/>
    public override string Name
        => _name;

    /// <inheritdoc/>
    public override Version Version
        => ThisModule.Version;

    /// <inheritdoc/>
    public override Guid InstanceId
        => _id;

    /// <inheritdoc/>
    public override PSHostUserInterface? UI
        => _ui;

    /// <inheritdoc/>
    public override CultureInfo CurrentCulture
    {
        get { lock (_lock) return _host.CurrentCulture; }
    }

    /// <inheritdoc/>
    public override CultureInfo CurrentUICulture
    {
        get { lock (_lock) return _host.CurrentUICulture; }
    }

    /// <inheritdoc/>
    public override PSObject PrivateData
    {
        get { lock (_lock) return _host.PrivateData; }
    }

    /// <inheritdoc/>
    public override bool DebuggerEnabled
    {
        get { lock (_lock) return _host.DebuggerEnabled; }
        set { lock (_lock) _host.DebuggerEnabled = value; }
    }

    /// <inheritdoc/>
    public override void EnterNestedPrompt()
    {
        lock (_lock) _host.EnterNestedPrompt();
    }

    /// <inheritdoc/>
    public override void ExitNestedPrompt()
    {
        lock (_lock) _host.ExitNestedPrompt();
    }

    /// <inheritdoc/>
    public override void NotifyBeginApplication()
    {
        lock (_lock) _host.NotifyBeginApplication();
    }

    /// <inheritdoc/>
    public override void NotifyEndApplication()
    {
        lock (_lock) _host.NotifyEndApplication();
    }

    /// <inheritdoc/>
    public override void SetShouldExit(int exitCode)
    {
        lock (_lock) _host.SetShouldExit(exitCode);
    }

    /// <summary>
    ///   Acquires an exclusive lock on the host.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This method blocks until the current thread acquires a lock.
    ///   </para>
    ///   <para>
    ///     It is safe for a thread to invoke this method if the thread already
    ///     holds a lock.  The thread continues to hold the lock until the
    ///     thread invokes <see cref="Unlock"/> once for each corresponding
    ///     invocation of <c>Lock</c>.
    ///   </para>
    /// </remarks>
    public void Lock()
    {
        Monitor.Enter(_lock);
    }

    /// <summary>
    ///   Releases an exclusive lock on the host.
    /// </summary>
    /// <remarks>
    ///   A thread that holds a lock continues to hold it until the thread
    ///   invokes <c>Unlock</c> once for each corresponding invocation of
    ///   <see cref="Lock"/>.
    /// </remarks>
    /// <exception cref="SynchronizationLockException">
    ///   The current thread does not hold an exclusive lock on the host.
    /// </exception>
    public void Unlock()
    {
        Monitor.Exit(_lock);
    }
}
