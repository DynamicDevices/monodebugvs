using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;

namespace DynamicDevices.MonoDebugVS
{
    // This class implments IDebugProgramProvider2. 
    // This registered interface allows the session debug manager (SDM) to obtain information about programs 
    // that have been "published" through the IDebugProgramPublisher2 interface.
    [ComVisible(true)]
    [Guid("62A511F0-3E9F-4625-B714-D6C9314B09BD")]
    public class AD7ProgramProvider : IDebugProgramProvider2
    {
        public AD7ProgramProvider()
        {
        }

        #region IDebugProgramProvider2 Members

        // Obtains information about programs running, filtered in a variety of ways.
        int IDebugProgramProvider2.GetProviderProcessData(enum_PROVIDER_FLAGS Flags, IDebugDefaultPort2 port, AD_PROCESS_ID ProcessId, CONST_GUID_ARRAY EngineFilter, PROVIDER_PROCESS_DATA[] processArray)
        {
            processArray[0] = new PROVIDER_PROCESS_DATA();

#if false
            if (EngineUtils.IsFlagSet((uint)Flags, (int)enum_PROVIDER_FLAGS.PFLAG_GET_PROGRAM_NODES))
            {
                // The debugger is asking the engine to return the program nodes it can debug. The 
                // sample engine claims that it can debug all processes, and returns exsactly one
                // program node for each process. A full-featured debugger may wish to examine the
                // target process and determine if it understands how to debug it.

                IDebugProgramNode2 node = (IDebugProgramNode2)(new AD7ProgramNode((int)ProcessId.dwProcessId));

                IntPtr[] programNodes = { Marshal.GetComInterfaceForObject(node, typeof(IDebugProgramNode2)) };

                IntPtr destinationArray = Marshal.AllocCoTaskMem(IntPtr.Size * programNodes.Length);
                Marshal.Copy(programNodes, 0, destinationArray, programNodes.Length);

                processArray[0].Fields = enum_PROVIDER_FIELDS.PFIELD_PROGRAM_NODES;
                processArray[0].ProgramNodes.Members = destinationArray;
                processArray[0].ProgramNodes.dwCount = (uint)programNodes.Length;

                return Constants.S_OK;
            }
#endif

            return Constants.S_FALSE;
        }

        // Gets a program node, given a specific process ID.
        int IDebugProgramProvider2.GetProviderProgramNode(enum_PROVIDER_FLAGS Flags, IDebugDefaultPort2 port, AD_PROCESS_ID ProcessId, ref Guid guidEngine, ulong programId, out IDebugProgramNode2 programNode)
        {
            // This method is used for Just-In-Time debugging support, which this program provider does not support
            programNode = null;
            return Constants.E_NOTIMPL;
        }

        int IDebugProgramProvider2.SetLocale(ushort wLangID)
        {
            return Constants.S_OK;
        }

        int IDebugProgramProvider2.WatchForProviderEvents(enum_PROVIDER_FLAGS Flags, IDebugDefaultPort2 port, AD_PROCESS_ID ProcessId, CONST_GUID_ARRAY EngineFilter, ref Guid guidLaunchingEngine, IDebugPortNotify2 ad7EventCallback)
        {
            return Constants.S_OK;
        }

        #endregion
    }
}
