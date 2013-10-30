using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace DynamicDevices.MonoDebugVS
{
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        #region Fields

        private DTE2 _thisApplication;
        private AddIn _thisAddin;

        #endregion

        #region Constructor
        
        public Connect()
        {
            
        }

        #endregion

        #region IDTExtensibility2 Members

        public void OnAddInsUpdate(ref Array custom)
        {
        }

        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>
        /// We get this when the add-in is loaded, and thus can register up our commands within the VS menu
        /// </summary>
        /// <param name="application"></param>
        /// <param name="connectMode"></param>
        /// <param name="addInInst"></param>
        /// <param name="custom"></param>
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            var menuName = "Debug";

            // Store handles to the application and the add-in
            _thisApplication = (DTE2)application;
            _thisAddin = (AddIn)addInInst;

            // Only add our custom command when setting up
            if (connectMode != ext_ConnectMode.ext_cm_UISetup)
                return;

            // Create registry keys (should have been done by installer but just in case...)
            RegistryHelper.SetupRegistry();

            // Handle different locales););););
            try
            {
                var resourceManager = new ResourceManager("MonoDebugVS.CommandBar", Assembly.GetExecutingAssembly());
                var cultureInfo = new CultureInfo(_thisApplication.LocaleID);
                string resourceName;

                if (cultureInfo.TwoLetterISOLanguageName == "zh")
                {
                    var parentCultureInfo = cultureInfo.Parent;
                    resourceName = String.Concat(parentCultureInfo.Name, menuName);
                }
                else
                {
                    resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, menuName);
                }

                // Update ourselves to the localised name
                menuName = resourceManager.GetString(resourceName);
            }
            catch
            {
            }

            // Get the main menu command bar
            var menuBarCommandBar = _thisApplication.CommandBars["MenuBar"];

            // Get the "Debug" command bar
            var debugCB = menuBarCommandBar.Controls[menuName];
            var debugPopup = (CommandBarPopup)debugCB;

            // Now add in our new control
            try
            {
                var contextGUIDS = new object[] { };
                var commands = (Commands2)_thisApplication.Commands;

                var status = (int) vsCommandStatus.vsCommandStatusSupported +
                             (int) vsCommandStatus.vsCommandStatusEnabled;

                //var style = (int) vsCommandStyle.vsCommandStylePictAndText;
                var style = (int)vsCommandStyle.vsCommandStyleText;

                var type = vsCommandControlType.vsCommandControlTypeButton;

                var command = commands.AddNamedCommand2(_thisAddin, "MonoDebug", "Debug with Mono", "Debugs the current project with Mono", false, 59, ref contextGUIDS, status, style, type);

                //Add a control for the command to the tools menu:
                if ((command != null) && (debugPopup!= null))
                {
                    command.AddControl(debugPopup.CommandBar, 1);
                }
            }
            catch (ArgumentException)
            {
            }
        }

        /// <summary>
        /// We get this when the add-in is unloaded
        /// </summary>
        /// <param name="removeMode"></param>
        /// <param name="custom"></param>
        public void OnDisconnection(ext_DisconnectMode removeMode, ref Array custom)
        {
        }

        public void OnStartupComplete(ref Array custom)
        {
        }

        #endregion

        #region IDTCommandTarget Members

        public void Exec(string cmdName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                if (cmdName == "DynamicDevices.MonoDebugVS.Connect.MonoDebug")
                {
                    // Here's where we lauch the debugger!
                    var sb = _thisAddin.DTE.Solution.SolutionBuild;
                
                    // Anything set?
                    if (sb.StartupProjects == null)
                        return;

                    // TODO: Should probably check this is a C# project here...

                    foreach (string projectName in (Array)sb.StartupProjects)
                    {
                        var project = _thisAddin.DTE.Solution.Item(projectName); 
	 
	                    var projectPath = (string)project.Properties.Item("FullPath").Value; 
	                    var outputPath = (string)project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value; 
	                    var outputFileName = (string)project.Properties.Item("OutputFileName").Value; 
	                    var debugSymbols = (bool)project.ConfigurationManager.ActiveConfiguration.Properties.Item("DebugSymbols").Value;

                        var fileName = projectPath + outputPath + outputFileName;

                        if(File.Exists(fileName))
                            LaunchDebugTarget(fileName);
                    }

                    return;
                }
            }
        }

        public void QueryStatus(string cmdName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (cmdName == "DynamicDevices.MonoDebugVS.Connect.MonoDebug")
                {

                    // TODO: Add in code somewhere to enable/disable the command based on whether we have a startup project
#if false
                    var sb = _thisAddin.DTE.Solution.SolutionBuild;

                    // Anything set?
                    if (sb.StartupProjects == null)
                        return;

                    if( ((Array)sb.StartupProjects).Length > 0)
                        statusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    else
                        statusOption = vsCommandStatus.vsCommandStatusSupported |
                                       vsCommandStatus.vsCommandStatusInvisible;
#endif
                    statusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;

                    return;
                }
            }
        }

        #endregion

        /// <summary>
        /// Launch an executible using the sample debug engine.
        /// </summary>
        private void LaunchDebugTarget(string filePath)
        {
            var sp = new Microsoft.VisualStudio.Shell.ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)_thisApplication);

            var dbg = (IVsDebugger)sp.GetService(typeof(SVsShellDebugger));

            var info = new VsDebugTargetInfo();
            info.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.dlo = DEBUG_LAUNCH_OPERATION.DLO_CreateProcess;

            info.bstrExe = filePath;
            info.bstrCurDir = System.IO.Path.GetDirectoryName(info.bstrExe);
            info.bstrArg = null; // no command line parameters
            info.bstrRemoteMachine = null; // debug locally
            info.fSendStdoutToOutputWindow = 0; // Let stdout stay with the application.
            info.clsidCustom = new Guid("{67e9d912-bd2d-49b7-9214-5d2595a39a5f}"); // Set the launching engine the sample engine guid
            info.grfLaunch = 0;

            IntPtr pInfo = System.Runtime.InteropServices.Marshal.AllocCoTaskMem((int)info.cbSize);
            System.Runtime.InteropServices.Marshal.StructureToPtr(info, pInfo, false);

            try
            {
                dbg.LaunchDebugTargets(1, pInfo);
            }
            finally
            {
                if (pInfo != IntPtr.Zero)
                {
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(pInfo);
                }
            }

        }

    }
}
