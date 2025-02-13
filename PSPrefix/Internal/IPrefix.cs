// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

namespace PSPrefix.Internal;

internal interface IPrefix
{
    int Length { get; }

    void Write(PSHostUserInterface ui);
}
