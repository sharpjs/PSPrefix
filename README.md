## About

The **PSPrefix** PowerShell module improves the output of long-running, potentially parallel tasks.

PSPrefix provides three cmdlets:

- **Show-Prefixed** – prefixes output with a custom header.
- **Show-Elapsed** – prefixes output with an elapsed-time header.
- **Get-SynchronizedHost** – makes the current `$Host` shareable across threads.

![An example showing use and output of Show-Prefixed nested within Show-Elapsed and invoking various Write cmdlets](https://raw.githubusercontent.com/sharpjs/PSPrefix/main/img/example.png)

## Status

[![Build](https://github.com/sharpjs/PSPrefix/workflows/Build/badge.svg)](https://github.com/sharpjs/PSPrefix/actions)
[![Build](https://img.shields.io/badge/coverage-100%25-brightgreen.svg)](https://github.com/sharpjs/PSPrefix/actions)
[![PSGallery](https://img.shields.io/powershellgallery/v/TaskHost.svg)](https://www.powershellgallery.com/packages/TaskHost)
[![PSGallery](https://img.shields.io/powershellgallery/dt/TaskHost.svg)](https://www.powershellgallery.com/packages/TaskHost)

## Installation

To install in PowerShell 7.2 or later:

```pwsh
Install-Module PSPrefix -AllowPrerelease
```

To update:

```pwsh
Update-Module PSPrefix -AllowPrerelease
```

## Usage

### The Basics

To prefix command output with an abitrary string, use the `Show-Prefix` cmdlet.

```pwsh
Show-Prefixed "Foo" {
    # your commands here
}
```

![Output of the Show-Prefixed example above](https://raw.githubusercontent.com/sharpjs/PSPrefix/main/img/output-prefixed.png)

To prefix command output with the elapsed time, use the `Show-Elapsed` cmdlet.

```pwsh
Show-Elapsed {
    # your commands here
}
```

![Output of the Show-Elapsed example above](https://raw.githubusercontent.com/sharpjs/PSPrefix/main/img/output-elapsed.png)

The cmdlets can be combined.

```pwsh
Show-Elapsed {
    Show-Prefix Foo {
        Show-Prefix Bar {
            # your commands here
        }
    }
}
```

![Output of the combined example above](https://raw.githubusercontent.com/sharpjs/PSPrefix/main/img/output-combined.png) 

### What Gets Prefixed

By default, this module's cmdlets affect the following output streams:

\#| Name        | Write Cmdlet
--|-------------|----------------
1 | Success     | `Write-Output`
2 | Error       | `Write-Error`
3 | Warning     | `Write-Warning`
4 | Verbose     | `Write-Verbose`
5 | Debug       | `Write-Debug`
6 | Information | `Write-Host`

The cmdlets do not affect the Progress stream.

To output objects (the Success stream) unaltered, use the `-PassThru` switch to
exempt that stream from prefix behavior.

```pwsh
Show-Prefixed Foo -PassThru {
    Write-Host   "This is prefixed"
    Write-Object "This is not prefixed"
}
```

![Output of the -PassThru parameter example above](https://raw.githubusercontent.com/sharpjs/PSPrefix/main/img/output-passthru.png) 

### The Clean Slate

The code between the `{` and `}` (the script block) runs in a new, clean
PowerShell environment (runspace).  No state from the invoking script is
visible in the script block.  Specifically:

- The current directory (location) is at its default value.
- Built-in PowerShell variables have their default values.
- No other variables are defined, except as shown below.
- No modules are imported, except as shown below.
- No profile scripts are run.

To pre-import modules for the script block to use, specify one or more modules
via the `-Module` parameter.

```pwsh
Show-Prefixed Foo -Module A, B {
    # modules A and B are available here
}
```

To copy variable values from the invoking script into the script block, specify
one or more variables via the `-Variable` parameter.

```pwsh
$x = 1337
$y =   42

Show-Prefixed Foo -Module x, y {
    # $x and $y are visible here
}
```

The script block is free to import modules, define variables, and perform other
setup as needed.  Becuase the script block runs isolated in its own runspace,
the script block's state changes do not affect the state of the invoking
script.

### Parallel Operations

PSPrefix is designed to add clarity to the output of long-running, parallel
operations.  The `Show-Elapsed` cmdlet provides relative time information.
The `Show-Prefix` cmdlet can differentiate output from separate tasks.
Together, the cmdlets can help to detangle webs of intermingled output from
multiple threads.

One trick is required when using the cmdlets in parallel multithreaded
scenarios.  The cmdlets work via custom `PSHost` implementations, but that
technique does not work inside `ForEach-Object -Parallel` script blocks, due to
how the latter cmdlet is implemented.  To work around this limitation, PSPrefix
provides a pair of features.

First, PSPrefix provides a `Get-SynchronizedHost` cmdlet that returns a wrapper
over the current `$Host` object that makes the host safe to share across
threads.

Second, the `Show-Elapsed` and `Show-Prefixed` cmdlets have a `-CustomHost`
parameter that accepts a host object to use instead of the current `$Host`
object.

To use `ForEach-Object -Parallel` with `Show-Elapsed` or `Show-Prefixed`, first
obtain a thread-safe host object outside the parallel script block.  Then pass
the thread-safe host object inside the parallel script block using the
`-CustomHost` parameter and the `$using:` syntax.

```pwsh
Show-Elapsed {
    $h = Get-SynchronizedHost
    1..4 | ForEach-Object -Parallel {
        Show-Prefixed "Task $_" -CustomHost $using:h {
            Write-Host "example command output"
        }
    }
}
```

![Output of the combined parallel usage example above](https://raw.githubusercontent.com/sharpjs/PSPrefix/main/img/output-parallel.png) 

<!--
  Copyright Subatomix Research Inc.
  SPDX-License-Identifier: MIT
-->
