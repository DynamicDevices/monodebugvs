using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;

namespace DynamicDevices.MonoDebugVS
{

    // AD7Engine is the primary entrypoint object for the sample engine. 
    //
    // It implements:
    //
    // IDebugEngine2: This interface represents a debug engine (DE). It is used to manage various aspects of a debugging session, 
    // from creating breakpoints to setting and clearing exceptions.
    //
    // IDebugEngineLaunch2: Used by a debug engine (DE) to launch and terminate programs.
    //
    // IDebugProgram3: This interface represents a program that is running in a process. Since this engine only debugs one process at a time and each 
    // process only contains one program, it is implemented on the engine.
    //
    // IDebugEngineProgram2: This interface provides simultanious debugging of multiple threads in a debuggee.

    [ComVisible(true)]
    [Guid("B759E695-A6DB-4A84-A316-85E104AC3B49")]
    public class AD7Engine : IDebugEngine2, IDebugEngineLaunch2, IDebugProgram3, IDebugEngineProgram2, IDebugSymbolSettings100
    {
        #region Fields

        /// <summary>
        /// ID of the debugger
        /// </summary>
        public const string Id = "{67e9d912-bd2d-49b7-9214-5d2595a39a5f}";

        #endregion

        #region Constructor

        public AD7Engine()
        {
            
        }

        #endregion

        #region IDebugEngine2 Members

        public int Attach(IDebugProgram2[] rgpPrograms, IDebugProgramNode2[] rgpProgramNodes, uint celtPrograms, IDebugEventCallback2 pCallback, enum_ATTACH_REASON dwReason)
        {
            throw new NotImplementedException();
        }

        public int CauseBreak()
        {
            throw new NotImplementedException();
        }

        public int ContinueFromSynchronousEvent(IDebugEvent2 pEvent)
        {
            throw new NotImplementedException();
        }

        public int CreatePendingBreakpoint(IDebugBreakpointRequest2 pBPRequest, out IDebugPendingBreakpoint2 ppPendingBP)
        {
            throw new NotImplementedException();
        }

        public int DestroyProgram(IDebugProgram2 pProgram)
        {
            throw new NotImplementedException();
        }

        public int EnumPrograms(out IEnumDebugPrograms2 ppEnum)
        {
            Debug.Fail("This function is not called by the debugger");
            ppEnum = null;
            return Constants.E_NOTIMPL;
        }

        public int GetEngineId(out Guid pguidEngine)
        {
            pguidEngine = new Guid(Id);
            return Constants.S_OK;
        }

        public int RemoveAllSetExceptions(ref Guid guidType)
        {
            throw new NotImplementedException();
        }

        public int RemoveSetException(EXCEPTION_INFO[] pException)
        {
            throw new NotImplementedException();
        }

        public int SetException(EXCEPTION_INFO[] pException)
        {
            throw new NotImplementedException();
        }

        public int SetLocale(ushort wLangID)
        {
            throw new NotImplementedException();
        }

        public int SetMetric(string pszMetric, object varValue)
        {
            throw new NotImplementedException();
        }

        public int SetRegistryRoot(string pszRegistryRoot)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugEngineLaunch2 Members

        public int CanTerminateProcess(IDebugProcess2 pProcess)
        {
            throw new NotImplementedException();
        }

        public int LaunchSuspended(string pszServer, IDebugPort2 pPort, string pszExe, string pszArgs, string pszDir, string bstrEnv, string pszOptions, enum_LAUNCH_FLAGS dwLaunchFlags, uint hStdInput, uint hStdOutput, uint hStdError, IDebugEventCallback2 pCallback, out IDebugProcess2 ppProcess)
        {
            throw new NotImplementedException();
        }

        public int ResumeProcess(IDebugProcess2 pProcess)
        {
            throw new NotImplementedException();
        }

        public int TerminateProcess(IDebugProcess2 pProcess)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProgram3 Members

        public int Attach(IDebugEventCallback2 pCallback)
        {
            Debug.Fail("This function is not called by the debugger");

            return Constants.E_NOTIMPL;
        }

        public int CanDetach()
        {
            throw new NotImplementedException();
        }

        public int Continue(IDebugThread2 pThread)
        {
            throw new NotImplementedException();
        }

        public int Detach()
        {
            throw new NotImplementedException();
        }

        public int EnumCodeContexts(IDebugDocumentPosition2 pDocPos, out IEnumDebugCodeContexts2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int EnumCodePaths(string pszHint, IDebugCodeContext2 pStart, IDebugStackFrame2 pFrame, int fSource, out IEnumCodePaths2 ppEnum, out IDebugCodeContext2 ppSafety)
        {
            throw new NotImplementedException();
        }

        public int EnumModules(out IEnumDebugModules2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int EnumThreads(out IEnumDebugThreads2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int Execute()
        {
            Debug.Fail("This function is not called by the debugger");

            return Constants.E_NOTIMPL;
        }

        public int ExecuteOnThread(IDebugThread2 pThread)
        {
            throw new NotImplementedException();
        }

        public int GetDebugProperty(out IDebugProperty2 ppProperty)
        {
            throw new NotImplementedException();
        }

        public int GetDisassemblyStream(enum_DISASSEMBLY_STREAM_SCOPE dwScope, IDebugCodeContext2 pCodeContext, out IDebugDisassemblyStream2 ppDisassemblyStream)
        {
            throw new NotImplementedException();
        }

        public int GetENCUpdate(out object ppUpdate)
        {
            throw new NotImplementedException();
        }

        public int GetEngineInfo(out string pbstrEngine, out Guid pguidEngine)
        {
            throw new NotImplementedException();
        }

        public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
        {
            throw new NotImplementedException();
        }

        public int GetName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int GetProcess(out IDebugProcess2 ppProcess)
        {
            Debug.Fail("This function is not called by the debugger");
            ppProcess = null;
            return Constants.E_NOTIMPL;
        }

        public int GetProgramId(out Guid pguidProgramId)
        {
            throw new NotImplementedException();
        }

        public int Step(IDebugThread2 pThread, enum_STEPKIND sk, enum_STEPUNIT Step)
        {
            throw new NotImplementedException();
        }

        public int Terminate()
        {
            throw new NotImplementedException();
        }

        public int WriteDump(enum_DUMPTYPE DUMPTYPE, string pszDumpUrl)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugEngineProgram2 Members

        public int Stop()
        {
            throw new NotImplementedException();
        }

        public int WatchForExpressionEvaluationOnThread(IDebugProgram2 pOriginatingProgram, uint dwTid, uint dwEvalFlags, IDebugEventCallback2 pExprCallback, int fWatch)
        {
            throw new NotImplementedException();
        }

        public int WatchForThreadStep(IDebugProgram2 pOriginatingProgram, uint dwTid, int fWatch, uint dwFrame)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugSymbolSettings100 Members

        public int SetSymbolLoadState(int bIsManual, int bLoadAdjacentSymbols, string bstrIncludeList, string bstrExcludeList)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}