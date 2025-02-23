// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

[TestFixture]
public class LockScopeTests
{
    [Test]
    public void ConstructAndDispose()
    {
        using var _ = new LockScope(@lock: new());
    }

    [Test]
    public void Dispose_LockNotTaken()
    {
        default(LockScope).Dispose();
    }
}
