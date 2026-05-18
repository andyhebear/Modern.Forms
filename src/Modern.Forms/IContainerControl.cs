// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms;

/// <summary>
///  Provides functionality for a control to parent other controls.
/// </summary>
public interface IContainerControl
{
    Control? ActiveControl { get; set; }

    bool ActivateControl (Control active);
}
