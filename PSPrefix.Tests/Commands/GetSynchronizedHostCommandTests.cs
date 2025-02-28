// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Commands;

using static ConsoleColor;
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

    [Test]
    public void UseInIntegrationTest()
    {
        const string ElapsedRegex = @"^\[\+[0-9]{2}:[0-9]{2}:[0-9]{2}\] $";

        RawUI.SetupProperty(u => u.ForegroundColor);
        RawUI.SetupProperty(u => u.BackgroundColor);

        RawUI.Object.ForegroundColor = White;
        RawUI.Object.BackgroundColor = Black;

        UI.Setup(u => u.Write    (DarkGray, Black, It.IsRegex(ElapsedRegex))).Verifiable();
        UI.Setup(u => u.Write    (DarkBlue, Black, "[Inner] "              )).Verifiable();
        UI.Setup(u => u.WriteLine(White,    Black, "Foo"                   )).Verifiable();

        var (output, exception) = Execute(
            Host.Object,
            // NOTE: Be careful to use the PSPrefix build under test, not any
            // version that happens to be installed on the machine
            """
            Show-Elapsed {
                $PSPrefix = Get-Module PSPrefix
                $SyncHost = Get-SynchronizedHost

                1 | ForEach-Object -Parallel {
                    Import-Module $using:PSPrefix
                    Show-Prefixed Inner { Write-Host Foo } -CustomHost $using:SyncHost
                }

            } -Module (Get-Module PSPrefix)
            """
        );

        exception   .ShouldBeNull();
        output.Count.ShouldBe(0);
    }
}
