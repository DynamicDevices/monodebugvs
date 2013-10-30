using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace DynamicDevices.MonoDebugVS
{
    public class RegistryHelper
    {
        private static string _engine = "{B759E695-A6DB-4A84-A316-85E104AC3B49}";
        private static string _programprovider = "{62A511F0-3E9F-4625-B714-D6C9314B09BD}";
        private static string _portsupplier = "{708C1ECA-FF48-11D2-904F-00C04FA302A1}";

        public static void SetupRegistry()
        {
            //
            // Register up the COM server
            //
            var key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\VisualStudio\\10.0\\AD7Metrics\\Engine\\" + AD7Engine.Id);
            key.SetValue("CLSID", _engine);
            key.SetValue("ProgramProvider", _programprovider);
            key.SetValue("Attach", 1);
            key.SetValue("AddressBP", 0);
            key.SetValue("AutoSelectPriority", 4);
            key.SetValue("CallstackBP", 1);
            key.SetValue("Name", "Mono Debug Engine");
            key.SetValue("PortSupplier", _portsupplier);
            key.Close();

            key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\VisualStudio\\10.0\\AD7Metrics\\Engine\\" + AD7Engine.Id + "\\IncompatibleList");
            key.SetValue("guidCOMPlusNativeEng", "{92EF0900-2251-11D2-B72E-0000F87572EF}");
            key.SetValue("guidCOMPlusOnlyEng", "{449EC4CC-30D2-4032-9256-EE18EB41B62B}");
            key.SetValue("guidNativeOnlyEng", "{449EC4CC-30D2-4032-9256-EE18EB41B62B}");
            key.SetValue("guidScriptEng", "{F200A7E7-DEA5-11D0-B854-00A0244A1DE2}");
            key.Close();

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar +
                       "Visual Studio 2010\\Addins\\" + new FileInfo(Assembly.GetExecutingAssembly().Location).Name;

            key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\VisualStudio\\10.0\\CLSID\\" + _engine);
            key.SetValue("Assembly", "DynamicDevices.MonoDebugVS");
            key.SetValue("Class", "DynamicDevices.MonoDebugVS.AD7Engine");
            key.SetValue("InProcServer32", "c:\\windows\\system32\\mscoree.dll");
            key.SetValue("CodeBase", path);
            key.Close();

            key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\VisualStudio\\10.0\\CLSID\\" + _programprovider);
            key.SetValue("Assembly", "DynamicDevices.MonoDebugVS");
            key.SetValue("Class", "DynamicDevices.MonoDebugVS.AD7ProgramProvider");
            key.SetValue("InProcServer32", "c:\\windows\\system32\\mscoree.dll");
            key.SetValue("CodeBase", path);
            key.Close();
        }

        public static void CleanRegistry()
        {
            Registry.LocalMachine.DeleteSubKey("SOFTWARE\\Microsoft\\VisualStudio\\10.0\\AD7Metrics\\Engine" + AD7Engine.Id);
            Registry.LocalMachine.DeleteSubKey("SOFTWARE\\Microsoft\\VisualStudio\\10.0\\CLSID\\" + _engine);
            Registry.LocalMachine.DeleteSubKey("SOFTWARE\\Microsoft\\VisualStudio\\10.0\\CLSID\\" + _programprovider);
        }
    }
}
