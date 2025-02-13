# Copyright Subatomix Research Inc.
# SPDX-License-Identifier: MIT
@{
    # Identity
    GUID          = '8408e9c6-0492-43e1-8503-46dbf43e71ff'
    RootModule    = 'PSPrefix.dll'
    ModuleVersion = '{VersionPrefix}'

    # General
    Description = 'Prefixes output with the elapsed time or a custom header. Good for long-running, potentially parallel tasks.'
    Author      = 'Jeffrey Sharp'
    CompanyName = 'Subatomix Research Inc.'
    Copyright   = '{Copyright}'

    # Requirements
    CompatiblePSEditions = 'Core'
    PowerShellVersion    = '7.2'
    #RequiredModules     = @(...)
    #RequiredAssemblies  = @(...)

    # Initialization
    #ScriptsToProcess = @(...)
    #TypesToProcess   = @(...)
    #FormatsToProcess = @(...)
    #NestedModules    = @(...)

    # Exports
    # NOTE: Use empty arrays to indicate no exports.
    FunctionsToExport    = @()
    VariablesToExport    = @()
    AliasesToExport      = @()
    DscResourcesToExport = @()
    CmdletsToExport      = @(
        "Get-SynchronizedHost"
        "Show-Elapsed"
        "Show-Prefixed"
    )

    # Discoverability and URLs
    PrivateData = @{
        PSData = @{
            # Additional metadata
            Prerelease   = '{VersionSuffix}'
            ProjectUri   = 'https://github.com/sharpjs/PSPrefix'
            ReleaseNotes = "https://github.com/sharpjs/PSPrefix/blob/main/CHANGES.md"
            LicenseUri   = 'https://github.com/sharpjs/PSPrefix/blob/main/LICENSE.txt'
            IconUri      = 'https://github.com/sharpjs/PSPrefix/blob/main/icon.png'
            Tags         = @(
                "Output", "Elapsed", "Prefix", "Header",
                "PSEdition_Core", "Windows", "Linux", "MacOS"
            )
        }
    }
}
