using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Rhino.Proxy.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("we")]
[assembly: AssemblyProduct("Rhino.Proxy.Tests")]
[assembly: AssemblyCopyright("Copyright © we 2006")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e3132458-876c-46cf-8e31-a3a89df53c5f")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]


#if DOTNET2
#if STRONG
[assembly: InternalsVisibleTo("DynamicAssemblyProxyGen, PublicKey=0024000004800000940000000602000000240000525341310004000001000100fb4ff5a7c8bba6feb6a6b75b260cd57c1b8b85b63a45dedcb7081331740c870852af30abd2a74700cce1d7a01aeed019339db202e010ac808396b2922362877c6afc8993281092434a223b9920cac8ba409d138a97b73cd1baad813af450b886e3d7f5a09ee450d415145eb0524778a6bd1ae733fd2b6ceebfd151620534bcb7")]
#else
[assembly: InternalsVisibleTo("DynamicAssemblyProxyGen")]
#endif
#endif