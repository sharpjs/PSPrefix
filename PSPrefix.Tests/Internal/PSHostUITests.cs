// Copyright Subatomix Research Inc.
// SPDX-License-Identifier: MIT

using System.Linq.Expressions;
using System.Net;
using System.Security;

namespace PSPrefix.Internal;

public abstract class PSHostUITests : TestHarnessBase
{
    private PSHostUserInterface? _ui;

    protected PSHostUserInterface UI => _ui ??= CreateHostUI(InnerUI.Object);

    protected MockSequence                 Sequence   { get; }
    protected Mock<PSHostUserInterface>    InnerUI    { get; }
    protected Mock<PSHostRawUserInterface> InnerRawUI { get; }

    protected PSHostUITests()
    {
        Sequence   = new();
        InnerUI    = Mocks.Create<PSHostUserInterface>();
        InnerRawUI = Mocks.Create<PSHostRawUserInterface>();

        InnerUI.Setup(u => u.RawUI).Returns(InnerRawUI.Object);
    }

    protected abstract PSHostUserInterface CreateHostUI(PSHostUserInterface ui);

    protected static PSCredential MakePSCredential(string username, string password)
    {
        return new(username, MakeSecureString(password));
    }

    protected static SecureString MakeSecureString(string value)
    {
        var secureString = new NetworkCredential(null, value).SecurePassword;
        secureString.MakeReadOnly();
        return secureString;
    }

    protected void Expect(Expression<Action<PSHostUserInterface>> expression)
    {
        InnerUI
            .InSequence(Sequence)
            .Setup(expression)
            .Verifiable();
    }

    protected void Expect<T>(Expression<Func<PSHostUserInterface, T>> expression, T result)
    {
        InnerUI
            .InSequence(Sequence)
            .Setup(expression)
            .Returns(result)
            .Verifiable();
    }
}
