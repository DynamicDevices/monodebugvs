using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;

namespace MonoDebugVS
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

            // Handle different locales
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
                if (cmdName == "MonoDebugVS.Connect.MonoDebug")
                {
                    // Here's where we lauch the debugger!
                    var sb = _thisAddin.DTE.Solution.SolutionBuild;

                    // Anything set?
                    if (sb.StartupProjects == null)
                        return;

                    foreach (var project in (Array)sb.StartupProjects)
                    {
                        MessageBox.Show("Starting: " + project);
                    }

                    return;
                }
            }
        }

        public void QueryStatus(string cmdName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (cmdName == "MonoDebugVS.Connect.MonoDebug")
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
    }
}
