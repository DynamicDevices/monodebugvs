﻿
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Diagnostics;
using System.Globalization;

namespace DynamicDevices.MonoDebugVS
{
    public static class EngineUtils
    {
        public static string BuildCommandLine(string exe, string args)
        {
            string startQuote = "\"";
            string afterExe = "\"";
            
            if (exe.Length <= 0)
            {
                throw new ComponentException(Constants.E_WIN32_INVALID_NAME);
            }

            if (exe[0] == '\"')
            {
                startQuote = string.Empty;
                if (exe.Length == 1)
                {
                    throw new ComponentException(Constants.E_WIN32_INVALID_NAME);
                }

                // If there are any more quotes, it needs to be the last character
                int endQuote = exe.IndexOf('\"', 1);
                if (endQuote > 0)
                {
                    if (exe.Length == 2 || endQuote != exe.Length-1)
                    {
                        throw new ComponentException(Constants.E_WIN32_INVALID_NAME);
                    }
                    afterExe = string.Empty;
                }
            }
            else
            {
                // If it doesn't start with a quote, it shouldn't have any
                if (exe.IndexOf('\"') >= 0)
                {
                    throw new ComponentException(Constants.E_WIN32_INVALID_NAME);
                }
            }
            
            if (args == null)
            {
                args = "";
            }
            else if (args != String.Empty)
            {
                if (afterExe != String.Empty)
                    afterExe = "\" ";
                else
                    afterExe = " ";
            }            

            return String.Concat(startQuote, exe, afterExe, args);
        }

#if false
        public static string GetAddressDescription(DebuggedModule module, uint ip)
        {
            string location = ip.ToString("x8", CultureInfo.InvariantCulture);

            if (module != null)
            {
                string moduleName = System.IO.Path.GetFileName(module.Name);

                location = string.Concat(moduleName, "!", location);
            }

            return location;
        }
#endif

        public static void CheckOk(int hr)
        {
            if (hr != 0)
            {
                throw new ComponentException(hr);
            }
        }

        public static void RequireOk(int hr)
        {
            if (hr != 0)
            {
                throw new InvalidOperationException();
            }
        }

        public static int GetProcessId(IDebugProcess2 process)
        {
            AD_PROCESS_ID[] pid = new AD_PROCESS_ID[1];
            EngineUtils.RequireOk(process.GetPhysicalProcessId(pid));

            if (pid[0].ProcessIdType != (uint)enum_AD_PROCESS_ID.AD_PROCESS_ID_SYSTEM)
            {
                return 0;
            }

            return (int) pid[0].dwProcessId;            
        }

        public static int GetProcessId(IDebugProgram2 program)
        {
            IDebugProcess2 process;
            RequireOk(program.GetProcess(out process));

            return GetProcessId(process);
        }

        public static int UnexpectedException(Exception e)
        {
            Debug.Fail("Unexpected exception during Attach");
            return Constants.RPC_E_SERVERFAULT;
        }

        internal static bool IsFlagSet(uint value, int flagValue)
        {
            return (value & flagValue) != 0;
        }
    }
}
