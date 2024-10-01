﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Proligence.PowerShell.Provider.Vfs.Navigation;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Proligence.PowerShell.Provider")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("Proligence Orchard PowerShell")]
[assembly: AssemblyCopyright("Copyright © Proligence")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("819feb68-18f4-410c-834f-95112d41df87")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("0.1.*")]
[assembly: AssemblyFileVersion("0.1.0.0")]

// The assembly is not CLS-compliant

[assembly: CLSCompliant(false)]

// Expose internal members to unit tests project.

[assembly: InternalsVisibleTo("Orchard.Tests.Modules.PowerShell.Provider")]
[assembly: PowerShellExtensionContainer]