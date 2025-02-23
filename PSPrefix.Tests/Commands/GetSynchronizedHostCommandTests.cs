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

        exception   .ShouldBeNull();
        output.Count.ShouldBe(1);
        output[0]   .ShouldNotBeNull()
            .BaseObject.ShouldBeOfType<SynchronizedHost>()
            .UI        .ShouldNotBeNull()
            .WriteLine();
    }
}
