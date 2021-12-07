// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Security.Specs.for_SecurityDescriptor;

public class MySecurityAction : ISecurityAction
{
    readonly Func<object, bool> _canAuthorize;
    readonly Func<object, AuthorizeActionResult> _authorize;

    public MySecurityAction(Func<object, bool> canAuthorize, Func<object, AuthorizeActionResult> authorize)
    {
        _canAuthorize = canAuthorize;
        _authorize = authorize;
    }

    public IEnumerable<ISecurityTarget> Targets { get; }

    public void AddTarget(ISecurityTarget securityTarget)
    {
    }

    public bool CanAuthorize(object actionToAuthorize)
    {
        return _canAuthorize.Invoke(actionToAuthorize);
    }

    public AuthorizeActionResult Authorize(object actionToAuthorize)
    {
        return _authorize.Invoke(actionToAuthorize);
    }

    public string ActionType { get { return "MySecurityAction"; } }
}