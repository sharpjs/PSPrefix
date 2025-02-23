// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

/// <summary>
///   A scope that acquires a <c>System.Threading.Monitor</c> lock on
///   construction and releases the lock on disposal.
/// </summary>
internal readonly ref struct LockScope
{
    private readonly object _lock;
    private readonly bool   _taken;

    /// <summary>
    ///   Initializes a new <see cref="LockScope"/> instance, acquiring a lock
    ///   on the specified object.
    /// </summary>
    /// <param name="lock">
    ///   The object to lock.
    /// </param>
    public LockScope(object @lock)
    {
        Monitor.Enter(_lock = @lock, ref _taken);
    }

    /// <summary>
    ///   Releases the lock.
    /// </summary>
    /// <exception cref="SynchronizationLockException">
    ///   The lock has been released already.
    /// </exception>
    public void Dispose()
    {
        if (_taken) Monitor.Exit(_lock);
    }
}
