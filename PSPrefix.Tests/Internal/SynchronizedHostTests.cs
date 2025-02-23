// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

[TestFixture]
public class SynchronizedHostTests : PSHostTests
{
    private new SynchronizedHost Host => (SynchronizedHost) base.Host;

    [Test]
    public void Name_Get()
    {
        Host.Name.Should().Be("SynchronizedHost(MockHost)");
    }

    [Test]
    public void UI_Get_NotNull()
    {
        Host.UI.Should().BeOfType<SynchronizedHostUI>();
    }

    [Test]
    public void LockAndUnluck()
    {
        var s = new MockSequence();

        InnerHost.InSequence(s).Setup(h => h.SetShouldExit(1)).Verifiable();
        InnerHost.InSequence(s).Setup(h => h.SetShouldExit(2)).Verifiable();

        // Set up a competing thread
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var task = Task.Run(async () =>
        {
            await gate.Task;
            Host.SetShouldExit(2);
        });

        // Acquire a lock to prevent the competing thread from accessing the host
        Host.Lock(); // once
        Host.Lock(); // twice to test re-entrancy

        // Competing thread will call SetShouldExit(2), but that blocks until
        // the current thread releases the lock
        gate.SetResult();
        Thread.Yield();

        // Current thread can call SetShouldExit(1) immediately
        Host.SetShouldExit(1);

        // Release lock on host; competing thread's SetShouldExit(2) call happens
        Host.Unlock();
        Host.Unlock();

        // Let the chips fall
        task.GetAwaiter().GetResult();
    }

    [Test]
    public void Unlock_WithoutLock()
    {
        Assert.Throws<SynchronizationLockException>(Host.Unlock);
    }

    protected override PSHost CreateHost(PSHost host)
        => new SynchronizedHost(host);
}
