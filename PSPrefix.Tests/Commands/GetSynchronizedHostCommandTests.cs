// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Commands;

using static ScriptExecutor;

[TestFixture]
public class GetSynchronizedHostCommandTests : CommandTests
{
    [Test]
    public void Invoke()
    {
        UI.Setup(u => u.WriteLine()).Verifiable();

        var (output, exception) = Execute(Host.Object, "Get-SynchronizedHost");

        exception      .Should().BeNull();
        output         .Should().ContainSingle().Which.AssignTo(out var obj);
        obj            .Should().NotBeNull();
        obj!.BaseObject.Should().BeOfType<SynchronizedHost>().Which.AssignTo(out var host);
        host.UI        .Should().NotBeNull();

        host.UI!.WriteLine();

    }
}
