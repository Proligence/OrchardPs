// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerShellExtensionContainerAttribute.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Navigation
{
    using System;

    /// <summary>
    /// Marks assemblies which contain extensions for the PowerShell VFS.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class PowerShellExtensionContainerAttribute : Attribute
    {
    }
}