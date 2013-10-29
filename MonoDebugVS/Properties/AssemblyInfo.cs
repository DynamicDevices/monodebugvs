using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MonoDebugVS")]

#if DEBUG
[assembly: AssemblyDescription("Mono Debugging Visual Studio 2010 Plugin (DEBUG)")]
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyDescription("Mono Debugging Visual Studio 2010 Plugin (RELEASE)")]
[assembly: AssemblyConfiguration("RELEASE")]
#endif

[assembly: AssemblyCompany("Dynamic Devices Ltd")]
[assembly: AssemblyProduct("MonoDebugVS")]
[assembly: AssemblyCopyright("Copyright © Dynamic Devices Ltd 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("95d24755-4d5c-4ed6-ba7b-963db4231e74")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
